using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class HomeworkSubmissionReadRepository : IHomeworkSubmissionReadRepository
{
    private readonly AppReadDbContext _context;

    public HomeworkSubmissionReadRepository(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HomeworkSubmissionRead>> GetByHomeworkIdAsync(string homeworkId)
    {
        return await _context.HomeworkSubmissionsRead
            .Where(hs => hs.Homework_id == homeworkId)
            .ToListAsync();
    }

    public async Task<HomeworkSubmissionRead?> GetByStudentAndHomeworkAsync(string studentId, string homeworkId)
    {
        return await _context.HomeworkSubmissionsRead
            .FirstOrDefaultAsync(hs => hs.Student_id == studentId && hs.Homework_id == homeworkId);
    }
}
