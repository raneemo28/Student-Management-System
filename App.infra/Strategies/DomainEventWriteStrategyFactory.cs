using App.Application.Common.Events;
using App.domain.entity;

namespace App.infra.Strategies;

public class DomainEventWriteStrategyFactory
{
    private readonly Dictionary<string, IDomainEventWriteStrategy> _strategies;

    public DomainEventWriteStrategyFactory(
        StudentCreatedWriteStrategy studentCreated,
        StudentUpdatedWriteStrategy studentUpdated,
        StudentDeletedWriteStrategy studentDeleted,
        CourseCreatedWriteStrategy courseCreated,
        CourseUpdatedWriteStrategy courseUpdated,
        CourseDeletedWriteStrategy courseDeleted,
        EnrollmentCreatedWriteStrategy enrollmentCreated,
        EnrollmentDeletedWriteStrategy enrollmentDeleted,
        InstructorCreatedWriteStrategy instructorCreated,
        InstructorUpdatedWriteStrategy instructorUpdated,
        InstructorDeletedWriteStrategy instructorDeleted,
        CourseContentCreatedWriteStrategy courseContentCreated,
        CourseContentUpdatedWriteStrategy courseContentUpdated,
        CourseContentDeletedWriteStrategy courseContentDeleted,
        EmployeeCreatedWriteStrategy employeeCreated,
        EmployeeUpdatedWriteStrategy employeeUpdated,
        EmployeeDeletedWriteStrategy employeeDeleted,
        AdvertisementCreatedWriteStrategy advertisementCreated,
        AdvertisementUpdatedWriteStrategy advertisementUpdated,
        AdvertisementDeletedWriteStrategy advertisementDeleted,
        HomeworkCreatedWriteStrategy homeworkCreated,
        HomeworkUpdatedWriteStrategy homeworkUpdated,
        HomeworkDeletedWriteStrategy homeworkDeleted,
        HomeworkSubmissionCreatedWriteStrategy homeworkSubmissionCreated,
        HomeworkSubmissionDeletedWriteStrategy homeworkSubmissionDeleted,
        HomeworkSolutionCreatedWriteStrategy homeworkSolutionCreated,
        HomeworkSolutionDeletedWriteStrategy homeworkSolutionDeleted,
        HomeworkQuestionMarkCreatedWriteStrategy homeworkQuestionMarkCreated,
        HomeworkQuestionMarkDeletedWriteStrategy homeworkQuestionMarkDeleted)
    {
        _strategies = new Dictionary<string, IDomainEventWriteStrategy>(StringComparer.OrdinalIgnoreCase)
        {
            ["StudentCreated"] = studentCreated,
            ["StudentUpdated"] = studentUpdated,
            ["StudentDeleted"] = studentDeleted,
            ["CourseCreated"] = courseCreated,
            ["CourseUpdated"] = courseUpdated,
            ["CourseDeleted"] = courseDeleted,
            ["EnrollmentCreated"] = enrollmentCreated,
            ["EnrollmentDeleted"] = enrollmentDeleted,
            ["InstructorCreated"] = instructorCreated,
            ["InstructorUpdated"] = instructorUpdated,
            ["InstructorDeleted"] = instructorDeleted,
            ["CourseContentCreated"] = courseContentCreated,
            ["CourseContentUpdated"] = courseContentUpdated,
            ["CourseContentDeleted"] = courseContentDeleted,
            ["EmployeeCreated"] = employeeCreated,
            ["EmployeeUpdated"] = employeeUpdated,
            ["EmployeeDeleted"] = employeeDeleted,
            ["AdvertisementCreated"] = advertisementCreated,
            ["AdvertisementUpdated"] = advertisementUpdated,
            ["AdvertisementDeleted"] = advertisementDeleted,
            ["HomeworkCreated"] = homeworkCreated,
            ["HomeworkUpdated"] = homeworkUpdated,
            ["HomeworkDeleted"] = homeworkDeleted,
            ["HomeworkSubmissionCreated"] = homeworkSubmissionCreated,
            ["HomeworkSubmissionDeleted"] = homeworkSubmissionDeleted,
            ["HomeworkSolutionCreated"] = homeworkSolutionCreated,
            ["HomeworkSolutionDeleted"] = homeworkSolutionDeleted,
            ["HomeworkQuestionMarkCreated"] = homeworkQuestionMarkCreated,
            ["HomeworkQuestionMarkDeleted"] = homeworkQuestionMarkDeleted
        };
    }

    public bool TryGetStrategy(string eventType, out IDomainEventWriteStrategy? strategy)
    {
        return _strategies.TryGetValue(eventType, out strategy);
    }
}
