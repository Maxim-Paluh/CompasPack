using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.ViewModel
{
    public class ComputerReportViewModel : ViewModelBase, IDetailViewModel
    {
        private string _processorName;
        private string _baseBoardName;

        public ComputerReportViewModel()
        {

        }

        public string ProcessorName
        {
            get { return _processorName; }
            set
            {
                _processorName = value;
                OnPropertyChanged();
            }
        }
        public string BaseBoardName
        {
            get { return _baseBoardName; }
            set
            {
                _baseBoardName = value;
                OnPropertyChanged();
            }
        }
        public bool HasChanges()
        {
            throw new NotImplementedException();
        }

        public async Task LoadAsync(int? Id)
        {
            ManagementObjectSearcher processors = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            ManagementObjectSearcher baseBoards = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject processor in processors.Get())
            {

                foreach (PropertyData property in processor.Properties)
                {
                    if (property != null)
                    {
                        if (property.Name == "Name")
                            ProcessorName = property.Value.ToString();
                        if (property.Name == "MaxClockSpeed")
                            ProcessorName += $"{double.Parse(property.Value.ToString()) / 1000}(GHz)";
                    }

                }
            }

            foreach (ManagementObject baseBoard in baseBoards.Get())
            {
                foreach (PropertyData property in baseBoard.Properties)
                {
                    if (property != null)
                    {
                        if (property.Name == "Product")
                            BaseBoardName = property.Value.ToString();
                    }

                }
            }

            var temp = string.Empty;

            foreach (ManagementObject baseBoard in baseBoards.Get())
            {
                foreach (PropertyData property in baseBoard.Properties)
                {
                    if (property != null)
                    {
                        temp += $"{property.Name}: ";
                        if (property.Value != null)
                            try
                            {
                                temp += property.Value.ToString() + "\n";
                            }
                            catch (Exception)
                            {
                            }

                    }

                }
            }




            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");

            string graphicsCard = string.Empty;
            foreach (ManagementObject mo in searcher.Get())
            {
                foreach (PropertyData property in mo.Properties)
                {
                    if (property.Name == "Description")
                    {
                        graphicsCard = property.Value.ToString();
                    }
                }
            }
        }

        public void Unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
