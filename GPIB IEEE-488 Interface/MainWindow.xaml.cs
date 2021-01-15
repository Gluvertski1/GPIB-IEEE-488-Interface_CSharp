using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NationalInstruments.NI4882;
using System.Threading;

namespace GPIB_IEEE_488_Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool gpibFlag = false;
        public bool scpiFlag = false;
        private Device device;
         


        public MainWindow()
        {
            InitializeComponent();

            ___boardIDDrop_.Items.Add('0');
            ___boardIDDrop_.Items.Add('1');
            ___boardIDDrop_.Items.Add('2');
            ___boardIDDrop_.Items.Add('3');
            ___boardIDDrop_.Items.Add('4');
            ___boardIDDrop_.Items.Add('5');
            ___boardIDDrop_.Items.Add('6');
            ___boardIDDrop_.Items.Add('7');
            ___boardIDDrop_.Items.Add('8');
            ___boardIDDrop_.Items.Add('9');
            ___boardIDDrop_.Items.Add(Convert.ToString(10));
            ___boardIDDrop_.Items.Add(Convert.ToString(11));
            ___boardIDDrop_.Items.Add(Convert.ToString(12));
            ___boardIDDrop_.Items.Add(Convert.ToString(13));
            ___boardIDDrop_.Items.Add(Convert.ToString(14));
            ___boardIDDrop_.Items.Add(Convert.ToString(15));
            ___boardIDDrop_.Items.Add(Convert.ToString(16));

            ___DevIDDrop_.Items.Add('0');
            ___DevIDDrop_.Items.Add('1');
            ___DevIDDrop_.Items.Add('2');
            ___DevIDDrop_.Items.Add('3');
            ___DevIDDrop_.Items.Add('4');
            ___DevIDDrop_.Items.Add('5');
            ___DevIDDrop_.Items.Add('6');
            ___DevIDDrop_.Items.Add('7');
            ___DevIDDrop_.Items.Add('8');
            ___DevIDDrop_.Items.Add('9');
            ___DevIDDrop_.Items.Add(Convert.ToString(10));
            ___DevIDDrop_.Items.Add(Convert.ToString(11));
            ___DevIDDrop_.Items.Add(Convert.ToString(12));
            ___DevIDDrop_.Items.Add(Convert.ToString(13));
            ___DevIDDrop_.Items.Add(Convert.ToString(14));
            ___DevIDDrop_.Items.Add(Convert.ToString(15));
            ___DevIDDrop_.Items.Add(Convert.ToString(16));

            //___boardIDDrop_.Items.AddRange.SerialPort.GetPortNames();
            
            
        }

        private void ___clrBtn__Click(object sender, RoutedEventArgs e)
        {
            //textBox1.Clear();
            listbox1.Items.Clear();
        }

        private void ___gpibRadio__Checked(object sender, RoutedEventArgs e)
        {
            gpibFlag = true;
            scpiFlag = false;
        }

        private void ___scpiRadio__Checked(object sender, RoutedEventArgs e)
        {
            scpiFlag = true;
            gpibFlag = false;
        }

        private void ___openBtn__Click(object sender, RoutedEventArgs e)
        {
            if(gpibFlag != true && scpiFlag != true)
            {
                MessageBox.Show("Please select either 196 or 2000!");
            }
            else
            {
                try
                {
                    if (___boardIDDrop_.SelectedItem != null && ___DevIDDrop_.SelectedItem != null)
                    {
                        int currentSecondaryAddress = 0;
                        int boardID = Convert.ToInt32(___boardIDDrop_.Text);
                        int devAddr = Convert.ToInt32(___DevIDDrop_.Text);

                        device = new Device((int)boardID, (byte)devAddr, (byte)currentSecondaryAddress);
                    }
                    else
                    {
                         MessageBox.Show("Please select a GPIB Board ID and Device Address!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                // disbling the user from selecting other things while operating.
                ___boardIDDrop_.IsEnabled = false;
                ___DevIDDrop_.IsEnabled = false;
                ___gpibRadio_.IsEnabled = false;
                ___scpiRadio_.IsEnabled = false;
                ___openBtn_.IsEnabled = false;
            }

            

            
        }

        private void ___closeBtn__Click(object sender, RoutedEventArgs e)
        {
            try
            {
                device.Dispose();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // enabling the inputs.
            ___boardIDDrop_.IsEnabled = true;
            ___DevIDDrop_.IsEnabled = true;
            ___gpibRadio_.IsEnabled = true;
            ___scpiRadio_.IsEnabled = true;
            ___openBtn_.IsEnabled = true;
        }

        private void ___boardIDDrop__SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ___DevIDDrop__SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ___readBtn__Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gpibFlag == true)
                {
                    // 1) need to set to DC volt auto range
                    // 2) set trigger to initiate req
                    // 3) clear buffer
                    // 4) take 100 samples
                    // 5) trigger reading with 'read?'
                    // 6) Then acquire all the samples from the Buffer with READ()

                    // resets
                    device.Write(ReplaceCommonEscapeSequences("*RST"));

                    // clear
                    device.Write(ReplaceCommonEscapeSequences("*CLS"));

                    // dc volts
                    device.Write(ReplaceCommonEscapeSequences("F0X"));

                    //auto range
                    device.Write(ReplaceCommonEscapeSequences("R0X"));

                    //set as continuous
                    device.Write(ReplaceCommonEscapeSequences("T4X"));

                    //set as 100 readings
                    device.Write(ReplaceCommonEscapeSequences("I1X"));

                    //readings without buffer location or prefixes
                    device.Write(ReplaceCommonEscapeSequences("G5X"));

                    // read from the buffer not the ADC
                    device.Write(ReplaceCommonEscapeSequences("B1X"));

                    Thread.Sleep(1000);

                    for (int i = 0; i < 100; i++)
                    {
                        // read
                        // read from store
                        
                        listbox1.Items.Add(device.ReadString());
                        listbox1.SelectedIndex = listbox1.Items.Count - 1;
                    }


                    

                }
                else if (scpiFlag == true)
                {
                    // do the keithly 2000 stuff here.

                    // create a string array 
                    byte[] data;

                    // resets
                    device.Write(ReplaceCommonEscapeSequences("*RST"));

                    // sets up for continuous acquisition.
                    device.Write(ReplaceCommonEscapeSequences("*CLS"));

                    //set to DC voltage readings
                    device.Write(ReplaceCommonEscapeSequences(":SENS:FUNC 'VOLT:DC'"));

                    // setting autorange for DC voltage
                    device.Write(ReplaceCommonEscapeSequences(":SENS:VOLT:DC:RANGE:AUTO ON"));

                    // set to show 4 digits
                    device.Write(ReplaceCommonEscapeSequences(":SENS:VOLT:DC:DIG 4"));

                    // sets the 
                    device.Write(ReplaceCommonEscapeSequences(":TRIG:COUN 1"));

                    // set to read the form 
                    device.Write(ReplaceCommonEscapeSequences(":FORM:ELEM READ"));

                    // set samp
                    device.Write(ReplaceCommonEscapeSequences(":SAMP:COUN 100"));

                    // set to 10 sampels per second
                    //device.Write(ReplaceCommonEscapeSequences(":SENS:VOLT:DC:NPLCycles 10"));

                    // intialize
                    device.Write(ReplaceCommonEscapeSequences("INIT"));

                    Thread.Sleep(1000);

                    // read the sata
                    device.Write(ReplaceCommonEscapeSequences(":TRAC:DATA?"));


                    data = device.ReadByteArray(2000);
                    string[] s = System.Text.Encoding.UTF8.GetString(data).Split(',');

                    for(int i = 0; i < s.Length; i++)
                    {
                        listbox1.Items.Add(s[i]);

                        listbox1.SelectedIndex = listbox1.Items.Count - 1;
                    }
   
                }
                else
                {
                    MessageBox.Show("Please select either 196 or 2000");
                }
            }
            catch(GpibException ex)
            {
                if (scpiFlag == true)
                {
                    device.Write(ReplaceCommonEscapeSequences(":SYST:ERR?"));
                    MessageBox.Show(ex.Message);
                }
                else
                {
                    MessageBox.Show(ex.Message);
                    //// show errors
                    //device.Write(ReplaceCommonEscapeSequences("U1X"));
                    //string show = device.ReadString();
                    //MessageBox.Show(show);

                }
            }
        }

        private string ReplaceCommonEscapeSequences(string s)
        {
            return s.Replace("\\n", "\n").Replace("\\r", "\r");
        }

        private string InsertCommonEscapeSequences(string s)
        {
            return s.Replace("\n", "\\n").Replace("\r", "\\r");
        }
    }
}
