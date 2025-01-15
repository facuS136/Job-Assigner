using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Job_Assigner
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, int> employees = new Dictionary<string, int>();                          // How many jobs can do at the time each employee
        private Dictionary<string, int> jobs = new Dictionary<string, int>();                               // How many employees the job needs at most 
        private Dictionary<string, List<string>> employeeJobs = new Dictionary<string, List<string>>();     // Which jobs can do each employee

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Employees List
        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            UserInputForm form = new UserInputForm("Ingrese el nombre del empleado:", "Ingrese la cantidad de trabajos:", "Añadir Empleado");
            // User gives the name of the new employee and the amount of jobs which that employee can do

            // check if the user confirmed, and if the input is valid or if already exists
            if (form.ShowDialog() == true && !string.IsNullOrWhiteSpace(form.Item1) && !employees.Keys.Contains(form.Item1))
            {
                employees[form.Item1] = form.Item2;               // add it to employees
                employeeJobs[form.Item1] = new List<string>();    // assign him an empty jobs list (the user will choose which jobs he is able to do)
                UpdateEmployeeList();                             // update employees table
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (lstEmployees.SelectedItem != null)          // if an employee was selected
            {
                string employeeName = ((KeyValuePair<string, int>)lstEmployees.SelectedItem).Key; // we get his name
                employees.Remove(employeeName);                             // remove it from employees
                employeeJobs.Remove(employeeName);                          // remove him from employeeJobs
                UpdateEmployeeList();                                       // update employees table
                lstEmployeeJobs.ItemsSource = null;
            }
        }

        private void UpdateEmployeeList()
        {
            lstEmployees.ItemsSource = null;
            lstEmployees.ItemsSource = employees;
        }
        #endregion

        #region Jobs List
        private void AddJob_Click(object sender, RoutedEventArgs e)
        {
            UserInputForm form = new UserInputForm("Ingrese el nombre del trabajo:", "Ingrese el numero de cupos:", "Añadir Trabajo");
            // User gives the name of the new job and the amount of employees needed at most

            // check if the user confirmed, and if the input is valid or if already exists
            if (form.ShowDialog() == true && !string.IsNullOrWhiteSpace(form.Item1) && !jobs.Keys.Contains(form.Item1))
            {
                jobs[form.Item1] = form.Item2;  // add it to jobs
                UpdateJobList();                // update jobs list
            }
        }
        private void DeleteJob_Click(object sender, RoutedEventArgs e)
        {
            if (lstJobs.SelectedItem != null)           // if a job is selected
            {
                string jobName = ((KeyValuePair<string, int>)lstJobs.SelectedItem).Key;
                jobs.Remove(jobName);                                       // remove it from jobs

                foreach (string employee in employeeJobs.Keys)
                {
                    if (employeeJobs[employee].Contains(jobName))           // if an employee had that job
                    {
                        employeeJobs[employee].Remove(jobName);             // delete it from him
                    }
                }

                UpdateJobList();                                            // update everything
                UpdateEmployeeJobsList();
            }
        }

        private void UpdateJobList()
        {
            lstJobs.ItemsSource = null;
            lstJobs.ItemsSource = jobs;
        }
        #endregion

        #region Employes Job List
        private void AddJobToEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (lstEmployees.SelectedItem != null)
            {
                string employeeName = ((KeyValuePair<string, int>)lstEmployees.SelectedItem).Key;   // get the selected employee name
                List<string> options = jobs.Keys.ToList();                                         
                options.RemoveAll(item => employeeJobs[employeeName].Contains(item));               // we give to the user the options of jobs for that employee
                SelectJobForm selectJobForm = new SelectJobForm(options);                           // user chooses the jobs he will give to the employee
                if (selectJobForm.ShowDialog() == true)                                             // if user confirmed
                {
                    foreach (var job in selectJobForm.SelectedJobs)
                    {
                        if (!employeeJobs[employeeName].Contains(job))
                        {
                            employeeJobs[employeeName].Add(job);                                    // employee will be able to do each job the user selected
                        }
                    }
                    UpdateEmployeeJobsList();
                }
            }
        }

        private void RemoveJobFromEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (lstEmployees.SelectedItem != null && lstEmployeeJobs.SelectedItem != null)      // if an employee and a job from the employee was selected
            {
                string employeeName = lstEmployees.SelectedItem.ToString();
                string jobName = lstEmployeeJobs.SelectedItem.ToString();
                employeeJobs[employeeName].Remove(jobName);                                     // we remove that job from the employee
                UpdateEmployeeJobsList();
            }
        }

        private void UpdateEmployeeJobsList()
        {
            if (lstEmployees.SelectedItem != null)
            {
                string employeeName = ((KeyValuePair<string, int>)lstEmployees.SelectedItem).Key;
                lstEmployeeJobs.ItemsSource = null;
                lstEmployeeJobs.ItemsSource = employeeJobs[employeeName];
            }
        }

        private void Employees_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateEmployeeJobsList();
        }
        #endregion


        private void btnAssignJobs_Click(object sender, RoutedEventArgs e)
        {
            string results = JobAssigner.DoAssignation(employees, jobs, employeeJobs);                              // we obtain the optimus assignation

            MessageBox.Show(results, "Asignación de Trabajos", MessageBoxButton.OK, MessageBoxImage.Information);   // print it on screen
        }

    }

    public class UserInputForm : Window
    {
        public string Item1;
        public int Item2;
        public UserInputForm(string text1, string text2, string title)
        {
            // This window is used to obtain a text input from the user.

            // create a window
            Width = 500;
            Height = 250;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Title = title;     // set the title given by the user

            // create the elements for the window stack
            StackPanel stackPanel = new StackPanel();                                           // a STACK for the elements
            TextBlock textBlock1 = new TextBlock() { Text = text1, Margin = new Thickness(10) };  // a TEXT BLOCK with the text given by the user
            TextBox textBox1 = new TextBox() { Width = 400, Margin = new Thickness(10) };        // a TEXT BLOCK for the user input
            TextBlock textBlock2 = new TextBlock() { Text = text2, Margin = new Thickness(10) };  // a TEXT BLOCK with the text given by the user
            TextBox textBox2 = new TextBox() { Width = 400, Margin = new Thickness(10) };        // a TEXT BLOCK for the user input
            Button confirmation = new Button()                                                  // a BUTTON to confirm actions
            {
                Content = "Ok",
                Width = 100,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            // set the confirmation button to set result has true and close
            confirmation.Click += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("El nombre no es valido", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(textBox2.Text, out _) || int.Parse(textBox2.Text) <= 0)
                {
                    MessageBox.Show("Debe insertar un valor numerico valido", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Item1 = textBox1.Text;
                Item2 = int.Parse(textBox2.Text);

                DialogResult = true;
                Close();
            };

            // add the elements to the window stack
            stackPanel.Children.Add(textBlock1);
            stackPanel.Children.Add(textBox1); 
            stackPanel.Children.Add(textBlock2);
            stackPanel.Children.Add(textBox2);
            stackPanel.Children.Add(confirmation);

            Content = stackPanel;    // add the stack panel to the window

            // if "Ok" is pressed, the text in the textBox is returned, else "" (nothing)
            //return prompt.ShowDialog() == true ? (textBox1.Text, int.Parse(textBox2.Text)) : ("",0);
        }
    }

    public class SelectJobForm : Window
    {
        public List<string> SelectedJobs { get; private set; } = new List<string>(); // job options selected by the user

        public SelectJobForm(List<string> jobs)
        {
            // we create a window
            Width = 400;
            Height = 300;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Title = "Seleccione Trabajos";

            StackPanel stackPanel = new StackPanel();       // a STACK for the elements
            ListBox listBox = new ListBox()                 // a LIST for the elements given by the user
            { 
                SelectionMode = SelectionMode.Multiple,     // user can select multiple jobs to be added to the employee
                ItemsSource = jobs                          // source given by the user
            };
            Button confirmation = new Button()              // a BUTTON to confirm actions
            { 
                Content = "Ok", 
                Width = 100, 
                Margin = new Thickness(10), 
                HorizontalAlignment = HorizontalAlignment.Right 
            };
            
            confirmation.Click += (sender, e) =>
            {
                SelectedJobs = listBox.SelectedItems.Cast<string>().ToList();
                DialogResult = true;
                Close();
            };

            stackPanel.Children.Add(listBox);
            stackPanel.Children.Add(confirmation);
            Content = stackPanel;
        }
    }
}


