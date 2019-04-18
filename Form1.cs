using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace TripCalculator
{
    public partial class Form1 : Form
    {
        private struct AmountOwed   // Used to track people who paid less than average
        {
            public string Name;
            public string OwedToName;
            public float Amount;
        }

        private struct AmountDifference // Used to track who is underpaid and who it overpaid
        {
            public string Name;
            public float Difference;
        }


        private Settings reg = new Settings(Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData), "TripCalculator", "Settings.xml"));


        private List<TripExpense> _listTripExpense = new List<TripExpense>();


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Restore to last window location
            string windowsPosition = reg.Read("WindowLocation");
            if (!windowsPosition.IsNullOrEmpty())
            {
                string[] sCoords = windowsPosition.Split(",".ToCharArray());
                this.Location = new Point(int.Parse(sCoords[0]), int.Parse(sCoords[1]));
                this.Width = int.Parse(sCoords[2]);
                this.Height = int.Parse(sCoords[3]);
            }

            dataGridView1.Columns.Clear();
            this.Refresh(); 

            // Preload data
            Thread thread = new Thread(() => 
            {
                PreFillData();
            });
            thread.Start();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Save the current window location
            string[] sCoords = new string[4];
            if (this.WindowState == FormWindowState.Normal)
            {
                sCoords[0] = this.Location.X.ToString();
                sCoords[1] = this.Location.Y.ToString();
                sCoords[2] = this.Width.ToString();
                sCoords[3] = this.Height.ToString();

                reg.Write("WindowLocation", string.Join(",", sCoords));
            }

            Application.Exit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options options = new Options();
            options.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.Show();
        }

        private void PreFillData()
        {
            _listTripExpense.Add(new TripCalculator.TripExpense { Name = "Louis", Meals = 5.75f, Hotels = 35f, TexiRides = 12.79f, PlaneTickets = 0f });
            _listTripExpense.Add(new TripCalculator.TripExpense { Name = "Carter", Meals = 12.0f, Hotels = 15.0f, TexiRides = 23.23f, PlaneTickets = 0f });
            _listTripExpense.Add(new TripCalculator.TripExpense { Name = "David", Meals = 10.0f, Hotels = 20.0f, TexiRides = 38.41f, PlaneTickets = 45.0f });


            var bindingList = new BindingList<TripExpense>(_listTripExpense);
            var source = new BindingSource(bindingList, null);
            SetDataSource(source);

        }

        private delegate void SetDataSourceDelegate(BindingSource item);
        private void SetDataSource(BindingSource dataSource)
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new SetDataSourceDelegate(SetDataSource), dataSource);
            }
            else
            {
                dataGridView1.DataSource = dataSource;
            }

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DisplayResults();
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;


            tb.KeyDown += new KeyEventHandler(dataGridViewTextBox_KeyDown);
            e.Control.KeyDown += new KeyEventHandler(dataGridViewTextBox_KeyDown);

            tb.KeyPress += new KeyPressEventHandler(dataGridViewTextBox_KeyPress);
            e.Control.KeyPress += new KeyPressEventHandler(dataGridViewTextBox_KeyPress);

        }

        private void dataGridViewTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (nonNumberEntered == true)
            {
                // Stop the character from being entered into the control since it is non-numerical.
                e.Handled = true;
            }

            nonNumberEntered = false;

            //if (e.KeyChar == (char)Keys.Enter)
            //{
            //    MessageBox.Show("You press Enter");
            //}
        }

        private void dataGridViewTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex > 0)
                CheckNumerEntered(e.KeyCode);   // Check to see if a numeric key is pressed

        }


        private bool nonNumberEntered = false;
        // Check if the numeric key is entered
        private void CheckNumerEntered(Keys keyCode)
        {
            // Initialize the flag to false.
            nonNumberEntered = false;

            // Determine whether the keystroke is a number from the top of the keyboard.
            if (keyCode < Keys.D0 || keyCode > Keys.D9)
            {
                // Determine whether the keystroke is a number from the keypad.
                if (keyCode < Keys.NumPad0 || keyCode > Keys.NumPad9)
                {
                    // Determine whether the keystroke is a backspace.
                    if (keyCode != Keys.Back && keyCode != Keys.OemPeriod && keyCode != Keys.Decimal)
                    {
                        // A non-numerical keystroke was pressed.
                        // Set the flag to true and evaluate in KeyPress event.
                        nonNumberEntered = true;
                    }
                }
            }
            //If shift key was pressed, it's not a number.
            if (Control.ModifierKeys == Keys.Shift)
            {
                nonNumberEntered = true;
            }
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            DisplayResults();

        }

        private async void DisplayResults()
        {
            List<AmountOwed> listAmountOwed = await CalculateIndividualCost();

            // Display results
            if (listAmountOwed.Count > 0)
            {
                listBox1.Items.Clear();
                foreach (var a in listAmountOwed)
                {
                    listBox1.Items.Add(a.Name + " owes " + a.OwedToName + " $" + a.Amount.ToString());
                }
            }
        }


        // There are really no asynchronous functions here. So the compiler warning remains
        private async Task<List<AmountOwed>> CalculateIndividualCost()
        {
            List<AmountOwed> listAmountOwed = new List<TripCalculator.Form1.AmountOwed>();

            // Get total expense for all 
            float totalExpense = 0.0f;
            int numberOfPeople = 0;
            foreach (var tripExpense in _listTripExpense)
            {
                if (tripExpense.Name.IsNullOrEmpty()) continue; // Make sure no blank data is used
                totalExpense += tripExpense.Total;
                numberOfPeople++;
            }
            float average = totalExpense / numberOfPeople;

            List<AmountDifference> listOverPaid = new List<TripCalculator.Form1.AmountDifference>();
            List<AmountDifference> listOwed = new List<TripCalculator.Form1.AmountDifference>();
            // Find out who owes and who is owed to
            foreach (var tripExpense in _listTripExpense)
            {
                if (tripExpense.Name.IsNullOrEmpty()) continue; // Make sure no blank data is used
                if (average - tripExpense.Total > .01)  // People underpaid
                    listOwed.Add(new AmountDifference() { Name = tripExpense.Name, Difference = average - tripExpense.Total });  // People who need to pay
                else
                    listOverPaid.Add(new AmountDifference() { Name = tripExpense.Name, Difference = tripExpense.Total - average}); // People who overpaid
            }

            // Sort by Difference
            listOwed = listOwed.OrderBy(o => o.Difference).ToList();
            listOverPaid = listOverPaid.OrderBy(o => o.Difference).ToList();
            foreach(var a in listOwed) // Start from the smallest owed amount to pay the largest overpaid amount
            {
                for (int i = listOverPaid.Count -1; i >= 0; i--)
                {
                    if (a.Difference < listOverPaid[i].Difference + .01)
                    {
                        listAmountOwed.Add(new AmountOwed() { Name = a.Name, OwedToName = listOverPaid[i].Name, Amount = (float)Math.Round(a.Difference, 2) });
                        // Update the owed amount since it's already paid by the person above
                        AmountDifference ad = listOverPaid[i];
                        ad.Difference = listOverPaid[i].Difference - (float)Math.Round(a.Difference, 2);
                        listOverPaid[i] = ad;
                    }
                    else
                    {
                        // To do:
                        // When the next amount owed is greater than the amount paid, happens when two or more people overpaid
                        // The payment will be split in two or more parts
                        // This program has alread demostrated the techniques I wanted to, so I will leave this part

                    }
                }

            }


            return listAmountOwed;
        }




    }
}
