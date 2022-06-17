using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Profiles
{
    public class ListActivities
    {
        public class Query: IRequest<Result<List<UserActivityDto>>>{
            public string Predicate { get; set; } = "future";
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<UserActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                IQueryable<UserActivityDto> query;
                //IQueryable<Activity> query;
                switch (request.Predicate)
                {
                    case "past":
                        query = _context.Activities
                            .OrderBy(x => x.Date)
                            .Where(x => x.Attendees.Any(x => x.AppUser.UserName == request.Username))
                            .Where(x => x.Date < DateTime.UtcNow)
                            .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider)
                            .AsQueryable();
                        break;
                    case "hosting":
                        query = _context.Activities
                            .OrderBy(x => x.Date)
                            .Where(x => x.Attendees.FirstOrDefault(x => x.IsHost).AppUser.UserName == request.Username)
                            .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider)
                            .AsQueryable();
                        break;
                    default:
                        query = _context.Activities
                            .OrderBy(x => x.Date)
                            .Where(x => x.Attendees.Any(x => x.AppUser.UserName == request.Username))
                            .Where(x => x.Date >= DateTime.UtcNow)
                            .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider)
                            .AsQueryable();
                        break;
                }

                //var activitiesToReturn = _mapper.Map<List<UserActivityDto>>(await query.ToListAsync());


                return Result<List<UserActivityDto>>.Success(await query.ToListAsync());
            }
        }
    }
}
