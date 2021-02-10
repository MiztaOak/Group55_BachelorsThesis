#create a brownian bridge from A to B
import numpy
import six
from matplotlib import pyplot




seed = 0
#antal jumps
N = 1000
#antal bakterier
M = 10

numpy.random.seed(seed)


def sample_path_batch(M, N):
    dt = 1.0 / N
    dt_sqrt = numpy.sqrt(dt)
    B = numpy.empty((M, N), dtype=numpy.float32)
    start = 5
    B[:, 0] = start
    for n in six.moves.range(N - 1):
        t = n * dt
        xi = numpy.random.randn(M) * dt_sqrt
        B[:, n + 1] = B[:, n] * (1 - dt / (1 - t)) + xi
    return B


B = sample_path_batch(M, N)
pyplot.plot(B.T)
pyplot.title('Brownian Bridge för en bakterie')
pyplot.show()