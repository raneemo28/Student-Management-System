using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class HomeworkQuestionMarkReadRepository : IHomeworkQuestionMarkReadRepository
{
    private readonly AppReadDbContext _context;

    public HomeworkQuestionMarkReadRepository(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HomeworkQuestionMarkRead>> GetBySubmissionIdAsync(string submissionId)
    {
        return await _context.HomeworkQuestionMarksRead
            .Where(hqm => hqm.Submission_id == submissionId)
            .ToListAsync();
    }
}
