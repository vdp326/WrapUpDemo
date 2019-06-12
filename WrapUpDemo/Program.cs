using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapUpDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<PersonModel> people = new List<PersonModel>
            {
                new PersonModel{ FirstName = "Vu", LastName = "Pham", Email = "vu@pham.com" },
                new PersonModel { FirstName = "Sue", LastName = "Pham", Email = "sue@pham.com"},
                new PersonModel { FirstName = "John", LastName = "Smith", Email = "john@pham.com"}
            };

            List<CarModel> car = new List<CarModel>
            {
                new CarModel{ Manufacturer = "Toyota", Model = "Corolla"},
                new CarModel{ Manufacturer = "Toyota", Model = "Highlander"},
                new CarModel{ Manufacturer = "Ford", Model = "Mustang"},
            };

            DataAccess<PersonModel> peopleData = new DataAccess<PersonModel>();

            peopleData.BadEntryFound += PeopleData_BadEntryFound;

            peopleData.SaveToCSV(people, @"C:\Users\vupha\Desktop\C#\timcorey\people.csv");

            DataAccess<CarModel> carData = new DataAccess<CarModel>();

            carData.BadEntryFound += CarData_BadEntryFound;
            carData.SaveToCSV(car, @"C:\Users\vupha\Desktop\C#\timcorey\cars.csv");


            //people.SaveToCSV(@"C:\Users\vupha\Desktop\C#\timcorey\people.csv");
            //car.SaveToCSV(@"C:\Users\vupha\Desktop\C#\timcorey\cars.csv");

            Console.ReadLine();
        }

        private static void CarData_BadEntryFound(object sender, CarModel e)
        {
            Console.WriteLine($"Bad Entry found for {e.Manufacturer} {e.Model});
        }

        private static void PeopleData_BadEntryFound(object sender, PersonModel e)
        {
            Console.WriteLine($"Bad Entry found for {e.FirstName} {e.LastName}");

        }
    }

    public class DataAccess<T> where T: new()
    {

        public event EventHandler<T> BadEntryFound;

        //generic extension method
        public void SaveToCSV(List<T> items, string filePath) 
        {
            List<string> rows = new List<string>();
            T entry = new T();
            var cols = entry.GetType().GetProperties();
            string row = "";

            foreach (var col in cols)
            {
                row += $",{col.Name}";
            }

            row = row.Substring(1);
            rows.Add(row);

            //this loop is looping through each items
            foreach (var item in items)
            {
                row = "";
                bool badWordDetected = false;

                //looping through each columns and get the value in each specific item
                foreach (var col in cols)
                {
                    string val = col.GetValue(item, null).ToString();
                    badWordDetected = BadWordDetector(val);

                    if(badWordDetected == true)
                    {
                        BadEntryFound?.Invoke(this, item);
                        break;
                    }

                    row += $",{val}";
                }

                if(badWordDetected == false)
                {
                    row = row.Substring(1);
                    rows.Add(row);
                }
                

               
            }

            File.WriteAllLines(filePath, rows);
        }

        private bool BadWordDetector(string stringToTest)
        {
            bool output = false;
            string lowerCaseTest = stringToTest.ToLower();
            
            if(lowerCaseTest.Contains("darn") || lowerCaseTest.Contains("heck"))
            {
                output = true;
            }

            return output;
        }
    }
}
