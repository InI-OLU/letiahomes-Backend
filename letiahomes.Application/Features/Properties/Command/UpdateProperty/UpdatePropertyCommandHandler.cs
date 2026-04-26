using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.UpdateProperty
{
    public class UpdatePropertyCommandHandler(IRepositoryManager repositoryManager) : IRequestHandler<UpdatePropertyCommand, ApiResult<PropertyResponse>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;

        public Task<ApiResult<PropertyResponse>> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
