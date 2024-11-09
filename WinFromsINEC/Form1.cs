using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFromsINEC
{
    public partial class frmINEC : Form // WinForms 4.8.1
    {
        #region Attributes
        // Los arreglos que se van a utilizar para almacenar la información de las personas
        // Esto no remplaza una base de datos, pero tiene el problema de hacer lectura secuenciaoles.
        // El archivo no viene sumarizado, por lo cual se deben hacer distinciones de 0-4, 5-9, 10-14, 15-19, 20-49 (a partir de los 20 si esta sumarizado de 20-49), 50-59, 60+
        static PopulationAge[] Age_Arr = new PopulationAge[40]; // Se fijan 40, a pesar de que no se sabe cuantas edades hay, pero se fija un número para que no haya problemas
        static int[] Male_Arr = { 0, 0, 0, 0 }; // Resumen de los grupos de personas acomodados por su edad, siendo estos hombres
        static int[] Female_Arr = { 0, 0, 0, 0 }; // Resumen de los grupos de personas acomodados por su edad, siendo estas mujeres
        static int[] Scholarship_Arr = { 0, 0, 0, 0, 0, 0, 0 }; // Resumen del total de personas por cada nivel deeducación

        // Todas los niveles de educación en un arreglos de strings
        static string[] Scholarship_Label_Arr = // Posiciones 16 en adelante en el ASCII.txt
        {
            "Primaria Completa", // Headings o Rotulos
            "Primaria Incompleta",
            "Secundaria Completa",
            "Secundaria Incompleta",
            "Universitaria Completa",
            "Universitaria Incompleta",
            "Sin Estudios"
        };

        //Contadores para el array
        static int Age_Index = 0;
        //Variables para saber los totales de hombres y muejeres en cada caso
        static int Male_Total = 0;
        static int Female_Total = 0;

        //Instancia de las utilidades
        UtilitiesINEC Census_Utils = new UtilitiesINEC();

        Thread Male_Age_Thread; // Hilo para generar los datos (grupo etario) de los hombres
        Thread Female_Age_Thread; // Hilo para generar los datos de las muejeres
        Thread Scholarship_Thread; // Hilo para generar los datos de la escolaridad/educación
        #endregion

        public frmINEC()
        {
            // inicializa los componentes
            InitializeComponent();
        }

        #region Buttons
        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            //Evento que sucede al darle click al botón de "Cargar archivo"
            //Del archivo de texto obtiene la información y pasa al DataGrid
            ReadFile();

            //Se llena el data grid view con la información del archivo plano
            FillPeopleDataGrid();

            //Se instancian los hilos e inicializa la función en la que va a comenzar a ejecutarse
            Male_Age_Thread = new Thread(new ThreadStart(CalculateMen));
            Female_Age_Thread = new Thread(new ThreadStart(CalculateWomen));
            Scholarship_Thread = new Thread(new ThreadStart(CalculateScholarship));

            // Se activa la barra de prograso para que muestre como van avanzando los hilos
            ActivateProgressBar(true, "Calculando", 1, 3, 1);

            //Checkea que los hilos no estén siendo ejecutados y en caso de que no sea así los ejecuta
            if (!Male_Age_Thread.IsAlive) // Si el hilo no está corriendo
                Male_Age_Thread.Start(); // Se inicia el hilo
            //En caso de que esté ya ejecutandose envía un mensaje para indicar esto
            else Census_Utils.MessageThread(Male_Age_Thread);

            if (!Female_Age_Thread.IsAlive)
                Female_Age_Thread.Start();
            else
                Census_Utils.MessageThread(Female_Age_Thread);

            if (!Scholarship_Thread.IsAlive)
                Scholarship_Thread.Start();
            else
                Census_Utils.MessageThread(Scholarship_Thread);

            //Se invoca la acción dentro del parentesis para que se ejecute por aparte y otros procesos no agarren espacios ocupados.
            gridAgeToGender.Invoke(new Action(() => // Inoke hace una proteccion para multiprogramacion, donde los elementos de R/W (escritura) deben ser protegidos.
            {
                while (Male_Age_Thread.ThreadState == ThreadState.Running) // Si el hilo esta corriendo debe esperar
                {
                    Thread.Sleep(1000); // Libreria de hilos y espera
                }

                while (Female_Age_Thread.ThreadState == ThreadState.Running)
                {
                    Thread.Sleep(1000);
                }

                // El tercer hilo no se contemplo, pero se puede agregar
                // De momento se infiere que si estos son los unicos que estan esperando, entonces el que resta esta trabajando.

                //Se envia un mensaje para que se sepa que ya se terminó de ejecutar el hilo
                Census_Utils.LogMessage("Thread execution finished");
                //Coloca los valores del datagridview en 0
                ActivateProgressBar(false, string.Empty, 0, 0, 0);
                //Llena los DGV de genero y de escolaridad
                FillGenderDataGrid(); // DataGrid Secundario de Edades y Genero
                FillScholarshipDataGrid(); // DataGrid de Escolaridad

            }));
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (Male_Age_Thread.ThreadState == ThreadState.Running)
            {
                Census_Utils.MessageThread(Male_Age_Thread, "is still running");
            }
            else if (Female_Age_Thread.ThreadState == ThreadState.Running)
            {
                Census_Utils.MessageThread(Female_Age_Thread, "is still running");
            }
            else if (Scholarship_Thread.ThreadState == ThreadState.Running)
            {
                Census_Utils.MessageThread(Scholarship_Thread, "is still running");
            }
            else
            {
                Application.Exit();
            }
        }
        #endregion

        public void ActivateProgressBar(bool status, string message, int min, int max, int steps)
        {
            //Se establece el estado de la barra, el mensaje que va a mostrar, el minimo, el máximo y los pasos que lleva
            //SI el status es True
            if (status)
            {
                //Muestra el mensaje en el label arriba de la barra de atreas y los muestra en pantalla
                lblProgress.Text = message;
                lblProgress.Visible = true;
                //Muestra el progress bar y le coloca los valores necesarios
                prgAge.Visible = true;
                prgAge.Minimum = min;
                prgAge.Maximum = max;
                prgAge.Value = min;
                prgAge.Step = steps;
            }
            else// si es False, oculta la progress bar y el label
            {
                lblProgress.Visible = false;
                prgAge.Visible = false;
            }
        }

        #region Extracción de datos del archivo plano
        public void ReadFile()
        {
            // El archivo plano tipo ASCII que no esta delimitado por comas sino por longitudes.
            // Se debe trabajar por posiciones y por ende se trabaj con arrays/arreglos.
            // Las dos primeras son las edades
            // Las otras 7 son la proyeccion de cantidad de hombres
            // Las otras 7 es la proyeccion de cantidad de mujeres
            // De la posicion 16 en adelante cada 6 corresponde a un grupo educacional.
            // El ASCII no requiere de un programa extre para verse -> C# StreamReader

            //Se obtiene la ruta rerlativa del archivo plano
            string fileName = @".\DB\Proyeccion_2025.txt"; // Los datos estan basados en porcentajes, puede que haya diferencias de 2 a 3 personas.
            //Se almacena la longitud del archivo, eso permite al progress bar mostrar correctamente como va avanzando
            int length = Convert.ToInt32(new FileInfo(fileName).Length);
            //Varieble contador
            int step = 0;
            //Se crea una variable para colocar los valores que hay en cada linea del archivo
            string line = string.Empty;

            try
            {
                //Se lee la información del archivo de texto
                var reader = new StreamReader(fileName);
                //Lee la primer linea del archivo de texto para verificar que tenga contenido
                line = reader.ReadLine();
                //Coloca la variable contador como el valor total del largo de esa linea que se obtuvo
                step = Convert.ToInt32(line.Length); // Cada vez que lee un registro, se obtiene la longitud de la linea para que el progress bar camine

                //Activa la barra de progreso
                ActivateProgressBar(true, "Reading File", 1, length, step);
                //Se reinicia el valor de la progres var
                prgAge.Refresh();

                //Mientras no sea el final del archivo
                while (line != null) // Mientras haya lineas en el archivo
                {
                    //Obtiene los datos que se encuentran en la linea
                    GetFileData(line);
                    //Lee la siguiente liena del archivo planoi
                    line = reader.ReadLine();
                    //Avanza la barra de progreso
                    prgAge.PerformStep();
                }
                //Cierra el lector del archivo plano y libera los recursos del sistema
                reader.Close();
                //Se establece el valor al máximo posible para que se sepa que se acabó el proceso
                prgAge.Value = length;
                //Se envia el mensaje de exito del proceso
                Census_Utils.LogMessage("File read successfully");
                //Se borra la barra para seguir trabajando
                ActivateProgressBar(false, string.Empty, 0, 0, 0);
            }
            catch (Exception ex)//Si hay algún error lo muestra en la consola
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public void GetFileData(string fileLine)
        {
            //Instancia de la clase edad y lee de acuerdo a las posiciones
            Age_Arr[Age_Index] = new PopulationAge();

            //Estos sub string funcionan de manera que se coloca el indice desde el que se comienza a leer
            //y el segundo argumento es la cantidad de espacio hacia la derecha que se leen

            //Los primeros numeros del 0 al 2 corresponden a la edad
            Age_Arr[Age_Index].Age = Convert.ToInt32(fileLine.Substring(0, 2));

            //Los numeros del 2 al 7 corresponden a la cantidad de hombres que tienen esa edad
            Age_Arr[Age_Index].Male = Convert.ToInt32(fileLine.Substring(2, 7));

            //Los numeros del 2 al 7 corresponden a la cantidad de hombres que tienen esa edad
            Age_Arr[Age_Index].Female = Convert.ToInt32(fileLine.Substring(9, 7));

            //Todas las personas que tengan la Primaria Completa
            Age_Arr[Age_Index].Scholarship[0] = Convert.ToInt32(fileLine.Substring(16, 6));

            //Todas las personas que tengan la "Primaria Incompleta"
            Age_Arr[Age_Index].Scholarship[1] = Convert.ToInt32(fileLine.Substring(22, 6));

            //Todas las personas que tengan la "Secundaria Completa"
            Age_Arr[Age_Index].Scholarship[2] = Convert.ToInt32(fileLine.Substring(28, 6));

            //Todas las personas que tengan la "Secundaria Incompleta"
            Age_Arr[Age_Index].Scholarship[3] = Convert.ToInt32(fileLine.Substring(34, 6));

            //Todas las personas que tengan la "Universitaria Completa"
            Age_Arr[Age_Index].Scholarship[4] = Convert.ToInt32(fileLine.Substring(40, 6));

            //Todas las personas que tengan la "Universitaria Incompleta"
            Age_Arr[Age_Index].Scholarship[5] = Convert.ToInt32(fileLine.Substring(46, 6));

            //Todas las personas que no tenga estudios
            Age_Arr[Age_Index].Scholarship[6] = Convert.ToInt32(fileLine.Substring(52, 6));

            //Lo que tiene almacenado en el Age_Arr[age_index] es el total de los hombres que había en esta linea
            Male_Total += Age_Arr[Age_Index].Male; // Acumulador de totales de hombres

            //Lo que tiene almacenado en el Age_Arr[age_index] es el total de las muejeres que había en esta linea
            Female_Total += Age_Arr[Age_Index].Female; // Acumulador de totales de mujeres

            //Se imprimen los valores obtenidos en la consola
            Console.WriteLine($"**** {Age_Arr[Age_Index].Age} ****"); // Bitacora en la ventana de salida / output
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[0]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[1]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[2]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[3]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[4]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[5]);
            Console.WriteLine(Age_Arr[Age_Index].Scholarship[6]);
            Console.WriteLine("**** UL ****");

            //Se aumenta el contaodr en 1 para que la siguiente vez que llegue lo pase a la siguiente linea
            Age_Index++;
        }
        #endregion

        #region calc datos Hombre, mujeres y escolaridad

        public void CalculateMen()
        {
            // Por el uso del invoke se protege al hilo para indicar con el que se está trabajando
            txtThreadId.Invoke(new Action(() =>
            {
                //Se coloca en el textbox el nombre del hilo que se esta manejando
                txtThreadId.Text = Male_Age_Thread.ManagedThreadId.ToString();
            }));

            //Ciclo for para el calculo
            for (int i = 0; i < Age_Arr.Length; i++)
            {
                //Si lo que está en la posición actual del arreglo no es nulo
                if (Age_Arr[i] != null)
                {
                    // Con el lock y el .syncRoot se asegura de que solo 1 hilo pueda estar modificando los valores en el interior del arreglo
                    lock (Male_Arr.SyncRoot)
                    {
                        
                        if ((Age_Arr[i].Age >= 0) && (Age_Arr[i].Age <= 4)) //En caso de que la edad sea de 0-4
                            Male_Arr[0] += Age_Arr[i].Male;
                        else if ((Age_Arr[i].Age >= 5) && (Age_Arr[i].Age <= 9)) //En caso de que la edad sea de 5-9
                            Male_Arr[1] += Age_Arr[i].Male;
                        else if ((Age_Arr[i].Age >= 10) && (Age_Arr[i].Age <= 14)) //En caso de que la edad sea de 10-14
                            Male_Arr[2] += Age_Arr[i].Male;
                        else if ((Age_Arr[i].Age >= 15) && (Age_Arr[i].Age <= 19)) //En caso de que la edad sea de 15-19
                            Male_Arr[3] += Age_Arr[i].Male;
                        //Se hacen las sumatorias del totaL y avanza al siguiente espacio del array
                    }
                }
            }

            //Imprime los valores obtenidos en la consola
            Console.WriteLine("Male"); // Bitacora en la ventana de salida / output (backend)
            Console.WriteLine(Male_Arr[0]);
            Console.WriteLine(Male_Arr[1]);
            Console.WriteLine(Male_Arr[2]);
            Console.WriteLine(Male_Arr[3]);

            // Por el uso del invoke se protege el avance de la progress bar
            prgAge.Invoke(new Action(() =>
            {
                prgAge.PerformStep();
            }));
        }

        public void CalculateWomen()
        {
            // Por el uso del invoke se protege al hilo para indicar con el que se está trabajando
            txtThreadId.Invoke(new Action(() =>
            {
                //Se coloca en el textbox el nombre del hilo que se esta manejando
                txtThreadId.Text = Female_Age_Thread.ManagedThreadId.ToString();
            }));

            for (int i = 0; i < Age_Arr.Length; i++)
            {
                //Si lo que está en la posición actual del arreglo no es nulo
                if (Age_Arr[i] != null)
                {
                    // Con el lock y el .syncRoot se asegura de que solo 1 hilo pueda estar modificando los valores en el interior del arreglo
                    lock (Female_Arr.SyncRoot)
                    {
                        if ((Age_Arr[i].Age >= 0) && (Age_Arr[i].Age <= 4)) //En caso de que la edad sea de 0-4
                            Female_Arr[0] += Age_Arr[i].Female;
                        else if ((Age_Arr[i].Age >= 5) && (Age_Arr[i].Age <= 9)) //En caso de que la edad sea de 5-9
                            Female_Arr[1] += Age_Arr[i].Female;
                        else if ((Age_Arr[i].Age >= 10) && (Age_Arr[i].Age <= 14)) //En caso de que la edad sea de 10-14
                            Female_Arr[2] += Age_Arr[i].Female;
                        else if ((Age_Arr[i].Age >= 15) && (Age_Arr[i].Age <= 19)) //En caso de que la edad sea de 15-19
                            Female_Arr[3] += Age_Arr[i].Female;
                    }
                }
            }

            //Imprime los valores obtenidos en la consola
            Console.WriteLine("Female");
            Console.WriteLine(Female_Arr[0]);
            Console.WriteLine(Female_Arr[1]);
            Console.WriteLine(Female_Arr[2]);
            Console.WriteLine(Female_Arr[3]);

            // Por el uso del invoke se protege el avance de la progress bar
            prgAge.Invoke(new Action(() =>
            {
                prgAge.PerformStep();
            }));
        }
        public void CalculateScholarship()
        {
            txtThreadId.Invoke(new Action(() => // Se coloca en el textbox el nombre del hilo que se esta manejando
            {
                txtThreadId.Text = Scholarship_Thread.ManagedThreadId.ToString();
            }));

            for (int i = 0; i < Age_Arr.Length; i++) // Obtiene longitud del vector de edades
            {
                if (Age_Arr[i] != null)
                {
                    lock (Scholarship_Arr.SyncRoot)
                    {
                        if ((Age_Arr[i].Age >= 0) && (Age_Arr[i].Age <= 4))
                            AddScholarship(0, Age_Arr[i].Age);
                        else if ((Age_Arr[i].Age >= 5) && (Age_Arr[i].Age <= 9))
                            AddScholarship(1, Age_Arr[i].Age);
                        else if ((Age_Arr[i].Age >= 10) && (Age_Arr[i].Age <= 14))
                            AddScholarship(2, Age_Arr[i].Age);
                        else if ((Age_Arr[i].Age >= 15) && (Age_Arr[i].Age <= 19))
                            AddScholarship(3, Age_Arr[i].Age);
                        else if (Age_Arr[i].Age == 20)
                            AddScholarship(4, 20);
                        else if (Age_Arr[i].Age == 50)
                            AddScholarship(5, 21);
                        else if (Age_Arr[i].Age == 60)
                            AddScholarship(6, 22);
                    }
                }
            }
        }
        public void AddScholarship(int index, int age_index)
        {
            //A partir de contador del ciclo for y el dontador de la posción en el arreglo de escolaridad
            for (int i = 0; i < Age_Arr[age_index].Scholarship.Length; i++) // Un vector de 7 posiciones por grupo etario
                Scholarship_Arr[index] += Age_Arr[age_index].Scholarship[i]; // Totales del acumulador
        }

        #endregion

        #region carga datos en los gridview
        public void FillPeopleDataGrid()
        {
            gridAgeToGender.Font = new Font("Arial", 10); // Manipulacion de DataGrid - FonFamily
            gridAgeToGender.Columns.Add("Age", "Age"); // Columnas a mostrar
            gridAgeToGender.Columns.Add("Male", "Male");
            gridAgeToGender.Columns.Add("Female", "Female");

            gridAgeToGender.Columns["Male"].DefaultCellStyle.Format = "#.##"; // Formato de los datos
            gridAgeToGender.Columns["Female"].DefaultCellStyle.Format = "#.##";

            gridAgeToGender.Columns["Male"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridAgeToGender.Columns["Female"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            gridAgeToGender.Columns["Age"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridAgeToGender.Columns["Male"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridAgeToGender.Columns["Female"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            for (int i = 0; i < Age_Arr.Length; i++) // Recorre el vector de edades
            {
                if (Age_Arr[i] != null) // Si no es nulo
                {
                    gridAgeToGender.Rows.Add(Age_Arr[i].Age, Age_Arr[i].Male, Age_Arr[i].Female); // Agrega los datos al DataGrid
                }
            }

            gridAgeToGender.Refresh(); // El primer refresh es muy basico, solo requiere trasladar los datos
        }

        public void WriteSubTotal(string heading, int maleTotal, int femaleTotal, bool status, int type)
        {
            //Si es verdadero
            if (status)
            {
                //Si el tipo es 1, o sea se esta agregando la info del DGV de genero
                if (type == 1)
                    gridGenderAge.Rows.Add(); // Añade una fila vacía
                else //Sino se esta gregando al DGV de escolaridad
                    gridScholarship.Rows.Add();// Añade una fila vacía
            }

            //Si el tipo es 1, o sea se esta agregando la info del DGV de genero
            if (type == 1)
            {
                gridGenderAge.Rows.Add(heading, maleTotal, femaleTotal, maleTotal + femaleTotal); //Añade la información enviada
                gridGenderAge.Rows.Add();// Añade una fila vacía
            }
            else //Sino se esta gregando al DGV de escolaridad
            {
                gridScholarship.Rows.Add(heading, string.Empty, maleTotal);
                gridScholarship.Rows.Add();
            }
        }

        public void FillGenderDataGrid()
        {
            //Se agrega el tipo de fuente que va a usar el data grid view
            gridGenderAge.Font = new Font("Arial", 10);
            //Se agregan las columnas que vamos a usar en el data grid view
            gridGenderAge.Columns.Add("Age", "Age");
            gridGenderAge.Columns.Add("Male", "Male");
            gridGenderAge.Columns.Add("Female", "Female");
            gridGenderAge.Columns.Add("Total", "Total");
            //Se les coloca el formato a las columnas para que aparezacan los numero de esa manera
            gridGenderAge.Columns["Female"].DefaultCellStyle.Format = "#.##";
            gridGenderAge.Columns["Male"].DefaultCellStyle.Format = "#.##";
            gridGenderAge.Columns["Total"].DefaultCellStyle.Format = "#.##";

            //Se alinea la letra en el centro y a la derecha de la celda
            gridGenderAge.Columns["Male"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridGenderAge.Columns["Female"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridGenderAge.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //Esto lo agregó Minor y hace que las celdas se acomoden automáticamenmte para llenar todo el espacio que cunre el data grid view sin pasarse de más
            gridGenderAge.Columns["Age"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridGenderAge.Columns["Male"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridGenderAge.Columns["Female"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridGenderAge.Columns["Total"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            for (int i = 0; i < Age_Arr.Length; i++)
            {
                //Si lo que está en la posición actual del arreglo no es nulo
                if (Age_Arr[i] != null)
                {
                    //Conmvierte la edad en un string
                    string ageStr = Age_Arr[i].Age.ToString();
                    //Si el contador es igual a 0 en la columna "Total" coloca los totales completos de hombres, de mujeres y el general
                    if (i == 0) // Si esta en el indice primario debe dar el total
                        WriteSubTotal("Total", Male_Total, Female_Total, false, 1);

                    if (Age_Arr[i].Age == 0)//Rango de 0 - 4
                        WriteSubTotal("0-4", Male_Arr[0], Female_Arr[0], false, 1);//Aquí ya estan almacenados lo totales de cada rango por lo que se añaden los datos
                    else if (Age_Arr[i].Age == 5) //Rango de 5 - 9
                        WriteSubTotal("5-9", Male_Arr[1], Female_Arr[1], true, 1);//Al ser true primero se añade una nueva fila en el DGV y luego añade la info
                    else if (Age_Arr[i].Age == 10) //Rango de 10 - 14
                        WriteSubTotal("10-14", Male_Arr[2], Female_Arr[2], true, 1);
                    else if (Age_Arr[i].Age == 15)//Rango de 15 - 19
                        WriteSubTotal("15-19", Male_Arr[3], Female_Arr[3], true, 1);
                    else if (Age_Arr[i].Age == 20)//Rango de 20 - 49
                    {
                        gridGenderAge.Rows.Add();
                        ageStr = "20-49";//Se incializa el ageStr como el valor del rango
                    }
                    else if (Age_Arr[i].Age == 50)//Rango de 50 - 59
                    {
                        gridGenderAge.Rows.Add();
                        ageStr = "50-59";//Se incializa el ageStr como el valor del rango
                    }
                    else if (Age_Arr[i].Age == 60)//Rango de 60 - 60+
                    {
                        gridGenderAge.Rows.Add();
                        ageStr = "60+";//Se incializa el ageStr como el valor del rango
                    }
                    //Se guarda el rango en primer lugar y los totales correspondientes
                    gridGenderAge.Rows.Add(ageStr, Age_Arr[i].Male, Age_Arr[i].Female, (Age_Arr[i].Male + Age_Arr[i].Female));
                }
            }
            //Se refresca el DGV para mostrar la información
            gridAgeToGender.Refresh();
        }

        public void FillScholarshipDataGrid()
        {
            //Se agrega el tipo de fuente que va a usar el data grid view
            gridScholarship.Font = new Font("Arial", 10);
            //Se agregan las columnas que vamos a usar en el data grid view
            gridScholarship.Columns.Add("Age", "Age");
            gridScholarship.Columns.Add("Scholarship", "Scholarship");
            gridScholarship.Columns.Add("Total", "Total");

            //Se les coloca el formato a las columnas para que aparezacan los numeros de esa manera
            gridScholarship.Columns["Total"].DefaultCellStyle.Format = "#.##";

            //Se alinea la letra en el centro y a la derecha de la celda
            gridScholarship.Columns["Age"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gridScholarship.Columns["Scholarship"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gridScholarship.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //Esto lo agregó Minor y hace que las celdas se acomoden automáticamenmte para llenar todo el espacio que cunre el data grid view sin pasarse de más
            gridScholarship.Columns["Age"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            gridScholarship.Columns["Scholarship"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            gridScholarship.Columns["Total"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            for (int i = 0; i < Age_Arr.Length; i++)
            {
                //Si lo que está en la posición actual del arreglo no es nulo
                if (Age_Arr[i] != null)
                {
                    //Si el contador es igual a 0 en la columna "Total" coloca el total general
                    if (i == 0)
                        WriteSubTotal("Total", Male_Total + Female_Total, 0, false, 2);

                    if (Age_Arr[i].Age == 0)//Rango de 0 - 4
                        WriteSubTotal("0-4", Scholarship_Arr[0], 0, false, 2); //Se agrega la primera linea y se deja un espacio abajo
                    //Como solo hay un total general se envia la información por Male_total
                    else if (Age_Arr[i].Age == 5)//Rango de 5 - 9
                        WriteSubTotal("5-9", Scholarship_Arr[1], 0, true, 2); //Se agrega una linea antes de este y se coloca la información
                    else if (Age_Arr[i].Age == 10)//Rango de 10 - 14
                        WriteSubTotal("10-14", Scholarship_Arr[2], 0, true, 2); //Se agrega una linea antes de este y se coloca la información
                    else if (Age_Arr[i].Age == 15)//Rango de 15 - 19
                        WriteSubTotal("15-19", Scholarship_Arr[3], 0, true, 2); //Se agrega una linea antes de este y se coloca la información
                    else if (Age_Arr[i].Age == 20)//Rango de 20 - 49
                        WriteSubTotal("20-49", Scholarship_Arr[4], 0, true, 2); //Se agrega una linea antes de este y se coloca la información
                    else if (Age_Arr[i].Age == 50)//Rango de 50 - 59
                        WriteSubTotal("50-59", Scholarship_Arr[5], 0, true, 2); //Se agrega una linea antes de este y se coloca la información
                    else if (Age_Arr[i].Age == 60)//Rango de 60+
                        WriteSubTotal("60+", Scholarship_Arr[6], 0, true, 2); //Se agrega una linea antes de este y se coloca la información

                    for (int j = 0; j < Age_Arr[i].Scholarship.Length; j++) // Un vector de 7 posiciones por grupo etario
                    {
                        //Se añaden los valores al data grid view de ecolaridad
                        gridScholarship.Rows.Add(Age_Arr[i].Age, Scholarship_Label_Arr[j], Age_Arr[i].Scholarship[j]);
                    }
                }
            }

            gridScholarship.Refresh();
        }

        #endregion

    }

    public class PopulationAge
    {
        //Clase que con la que se crean las instancias para el llenado del data grid view
        public int Age { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int[] Scholarship = { 0, 0, 0, 0, 0, 0, 0 };
    }

    public class UtilitiesINEC
    {
        // Message function that indicates that a thread is executing and the managed threadid
        //Función que envía un mensaje que indica que el hilo se está ejecutando y en envía un mesage relacionado con el hilo
        public void MessageThread(Thread thread)
        {
            MessageBox.Show($"Thread {thread.ManagedThreadId} is executing");
        }

        public void MessageThread(Thread thread, string message)
        {
            MessageBox.Show($"Thread {thread.ManagedThreadId} is executing {message}");
        }

        public void LogMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
