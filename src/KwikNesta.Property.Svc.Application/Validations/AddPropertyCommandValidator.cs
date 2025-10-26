using FluentValidation;
using KwikNesta.Property.Svc.Application.Commands;
using KwikNesta.Property.Svc.Application.Common.Extensions;

namespace KwikNesta.Property.Svc.Application.Validations
{
    internal class AddPropertyCommandValidator : AbstractValidator<AddPropertyCommand>
    {
        public AddPropertyCommandValidator()
        {
            RuleFor(p => p.Title).NotEmpty().WithMessage("Title is is a required field.")
                .MaximumLength(200).WithMessage("Maximum length for Title is 200 characters");
            RuleFor(p => p.Description).NotEmpty().WithMessage("Title is is a required field.")
                .MaximumLength(2000).WithMessage("Maximum length for Description is 2000 characters");
            RuleFor(p => p.Type).IsInEnum()
                .WithMessage("Invalid Property Type");

            RuleFor(p => p.Features).NotNull()
                .WithMessage("Property Feature is required");
            RuleFor(p => p.Location).NotNull()
                .WithMessage("Property Location is required.");
            RuleFor(p => p).Must(args => args.Location.IsValidLocation());
        }
    }
}