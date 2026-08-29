using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class HomeworkSubmissionRepository : Repository<HomeworkSubmission>, IHomeworkSubmissionRepository
{
    public HomeworkSubmissionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<HomeworkSubmission>> GetByHomeworkIdAsync(string homeworkId)
    {
        return await _dbSet
            .Where(hs => hs.Homework_id == homeworkId)
            .ToListAsync();
    }

    public async Task<HomeworkSubmission?> GetByStudentAndHomeworkAsync(string studentId, string homeworkId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(hs => hs.Student_id == studentId && hs.Homework_id == homeworkId);
    }
}
