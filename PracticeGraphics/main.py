import matplotlib.pyplot as plt

path = "..\\DirectProblem\\Results\\"

with open(path + 'emfs.txt', 'r') as file:
    frequencies = [float(x) for x in file.readline().split()]
    pointsZ = [float(x) for x in file.readline().split()]

    emfSin1 = [float(x) for x in file.readline().split()]
    emfSin2 = [float(x) for x in file.readline().split()]
    emfSin3 = [float(x) for x in file.readline().split()]
    emfSin4 = [float(x) for x in file.readline().split()]

    emfCos1 = [float(x) for x in file.readline().split()]
    emfCos2 = [float(x) for x in file.readline().split()]
    emfCos3 = [float(x) for x in file.readline().split()]
    emfCos4 = [float(x) for x in file.readline().split()]

sin_min = min([min(emfSin1), min(emfSin2), min(emfSin3), min(emfSin4)])

plt.plot(pointsZ, emfSin1, label=f'{frequencies[0]} MHz sin')
plt.plot(pointsZ, emfSin2, label=f'{frequencies[1]} MHz sin')
plt.plot(pointsZ, emfSin3, label=f'{frequencies[2]} MHz sin')
plt.plot(pointsZ, emfSin4, label=f'{frequencies[3]} MHz sin')

plt.xlabel('Z')
plt.ylabel('EMF')

#plt.xticks([-1e-2, -1e-10, -1e-20, 0, 1e-20, 1e-10, 1e-2])
plt.xlim(-5.5, -4.5)
#plt.ylim(sin_min, abs(sin_min))

plt.legend()

plt.show()

cos_min = min([min(emfCos1), min(emfCos2), min(emfCos3), min(emfCos4)])

plt.plot(pointsZ, emfCos1, label=f'{frequencies[0]} MHz cos')
plt.plot(pointsZ, emfCos2, label=f'{frequencies[1]} MHz cos')
plt.plot(pointsZ, emfCos3, label=f'{frequencies[2]} MHz cos')
plt.plot(pointsZ, emfCos4, label=f'{frequencies[3]} MHz cos')

plt.xlabel('Z')
plt.ylabel('EMF')

#plt.xticks([-1e-2, -1e-10, -1e-20, 0, 1e-20, 1e-10, 1e-2])
plt.xlim(-5.5, -4.5)
#plt.ylim(cos_min, abs(cos_min))

plt.legend()

plt.show()

with open(path + 'phaseDifferences.txt', 'r') as file:
    file.readline()
    file.readline()
    phaseDifferences1 = [float(x) for x in file.readline().split()]
    phaseDifferences2 = [float(x) for x in file.readline().split()]
    phaseDifferences3 = [float(x) for x in file.readline().split()]
    phaseDifferences4 = [float(x) for x in file.readline().split()]

plt.plot(pointsZ, phaseDifferences1, label=f'{frequencies[0]} MHz phase differences')
plt.plot(pointsZ, phaseDifferences2, label=f'{frequencies[1]} MHz phase differences')
plt.plot(pointsZ, phaseDifferences3, label=f'{frequencies[2]} MHz phase differences')
plt.plot(pointsZ, phaseDifferences4, label=f'{frequencies[3]} MHz phase differences')

plt.xlabel('Z')
plt.ylabel('EMF')

#plt.xticks([-1e-2, -1e-10, -1e-20, 0, 1e-20, 1e-10, 1e-2])
plt.xlim(-5.5, -4.5)

plt.legend()

plt.show()