using AA2_CS.Model.Entities;

public class BodyMetricService
{
    private readonly BodyMetricRepository _repository;

    public BodyMetricService(BodyMetricRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BodyMetric>> GetByUserIdAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<BodyMetric?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<BodyMetric> AddAsync(BodyMetric metric)
    {
        return await _repository.AddAsync(metric);
    }

    public async Task<BodyMetric> UpdateAsync(BodyMetric metric)
    {
        return await _repository.UpdateAsync(metric);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<BodyMetric?> GetLatestAsync(int userId)
    {
        return await _repository.GetLatestAsync(userId);
    }

    public float? CalculateBMI(float weightKg, float heightCm)
    {
        return _repository.CalculateBMI(weightKg, heightCm);
    }
}
