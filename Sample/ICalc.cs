using System.Threading.Tasks;

public interface ICalc {
    Task<int> Sum(int a, int b);
}