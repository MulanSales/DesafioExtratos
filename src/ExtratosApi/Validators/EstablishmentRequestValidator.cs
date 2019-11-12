using System;
using System.Globalization;
using ExtratosApi.Models.Request;
using FluentValidation;
using FluentValidation.Validators;

namespace ExtratosApi.Validators
{
    public class EstablishmentRequestValidator: AbstractValidator<EstablishmentRequest>
    {
        public EstablishmentRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty()
                .NotNull()
                .MaximumLength(50);
            RuleFor(request => request.Type)
                .NotEmpty()
                .NotNull()
                .MaximumLength(25);
        }
    }
}