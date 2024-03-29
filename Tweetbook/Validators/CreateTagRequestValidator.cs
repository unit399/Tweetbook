﻿using FluentValidation;
using Tweetbook.Contract.V1.Requests;

namespace Tweetbook.Validators
{
    public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
    {
        public CreateTagRequestValidator()
        {
            RuleFor(x => x.TagName).NotEmpty().Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}
