using AA2_CS.Model;

public class BodyMetricService
{
    private readonly BodyMetricRepository _repository;

    public BodyMetricService(BodyMetricRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BodyMetric>> GetByUserIdAsync(int userId)
        => await _repository.GetByUserIdAsync(userId);

    public async Task<BodyMetric?> GetByIdAsync(int id)
        => await _repository.GetByIdAsync(id);

    public async Task<BodyMetric> AddAsync(BodyMetric metric)
        => await _repository.AddAsync(metric);

    public async Task<BodyMetric> UpdateAsync(BodyMetric metric)
        => await _repository.UpdateAsync(metric);

    public async Task<bool> DeleteAsync(int id)
        => await _repository.DeleteAsync(id);

    public async Task<BodyMetric?> GetLatestAsync(int userId)
        => await _repository.GetLatestAsync(userId);

    public float? CalculateBMI(float weightKg, float heightCm)
    {
        if (heightCm <= 0) return null;
        var heightM = heightCm / 100;
        return weightKg / (heightM * heightM);
    }
}
