using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class HomeworkQuestionMarkRepository : Repository<HomeworkQuestionMark>, IHomeworkQuestionMarkRepository
{
    public HomeworkQuestionMarkRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<HomeworkQuestionMark>> GetBySubmissionIdAsync(string submissionId)
    {
        return await _dbSet
            .Where(hqm => hqm.Submission_id == submissionId)
            .ToListAsync();
    }
}
