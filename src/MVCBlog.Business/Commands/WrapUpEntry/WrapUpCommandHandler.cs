using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MVCBlog.Data;
using MVCBlog.Localization;

namespace MVCBlog.Business.Commands
{
    public class WrapUpCommandHandler :
        ICommandHandler<WrapUpCommand>
    {
        private readonly EFUnitOfWork unitOfWork;

        public WrapUpCommandHandler(EFUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(WrapUpCommand command)
        {

            var entry = this.unitOfWork.WrapUps
                .SingleOrDefault(b => b.Id == command.Entity.Id);
            entry = command.Entity;

            this.unitOfWork.WrapUps.Add(entry);

            await this.unitOfWork.SaveChangesAsync();
        }
    }
}
