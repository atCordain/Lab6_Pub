using System;

namespace Lab6_Pub
{
    internal class EventMessage : EventArgs
    {
        private string message;
        public EventMessage(string message) 
        {
            this.message = message;
        }

        public string Message { get => message; }

    }
}