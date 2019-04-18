using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCalculator
{
    class TripExpense
    {
        private float _meals = 0;
        private float _hotels = 0;
        private float _taxiRides = 0;
        private float _planeTickets = 0;
        private float _total = 0;

        public string Name { get; set; }
        public float Meals { get { return _meals; } set { _meals = value; CalculateTotal(); } }
        public float Hotels { get { return _hotels; } set { _hotels = value; CalculateTotal(); } }
        public float TaxiRides { get { return _taxiRides; } set { _taxiRides = value; CalculateTotal(); } }
        public float PlaneTickets { get { return _planeTickets; } set { _planeTickets = value; CalculateTotal(); } }
        public float Total { get { return _total; } }

        private void CalculateTotal()
        {
            _total =  _meals + _hotels + _taxiRides + _planeTickets;
        }
    }
}
