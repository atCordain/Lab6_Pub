using System;

namespace Lab6_Pub
{
    public class Patron
    {
        private string name;
        internal Random random = new Random();

        public Patron()
        {
            
        }

        public string PatronName { get => name; set => name = value; }


    }
}