using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NerdStore.Core.Messages
{
    public abstract class BaseCommand : Message, IRequest<bool>
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }

        public BaseCommand()
        {
            Timestamp = DateTime.Now;
        }

        public abstract bool EhValido();
    }
}
