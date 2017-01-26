using System;

namespace ObserverPattern
{
    public delegate void EventHandler();
    public class RabbitInstances
    {
        private int totalNumberOfInstances = 50;

        public event EventHandler WithoutInstances;

        public void CreateInstances()
        {
            //Creo las instancias necesarias hasta alcanzar 50
            while (totalNumberOfInstances < 50)
            {
                totalNumberOfInstances++;
            }

            Console.WriteLine(totalNumberOfInstances);
        }

        public void CloseSingleInstance()
        {
            totalNumberOfInstances--;
            Console.WriteLine(totalNumberOfInstances);
            CheckInstances();
        }

        private void CheckInstances()
        {
            if (totalNumberOfInstances < 5)
            {
                WithoutInstances();
            }
        }

        public int InstancesAvailable()
        {
            return totalNumberOfInstances;
        }
    }
}
