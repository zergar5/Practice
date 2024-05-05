import numpy as np
import matplotlib.pyplot as plt
from matplotlib.patches import Rectangle
import os
import re

# Функция для чтения и обработки данных из файла
def read_phase_differences_from_file(file_path):
    with open(file_path, 'r') as file:
        lines = file.readlines()
        frequencies = list(map(float, lines[0].split()))
        z_coordinate = list(map(float, lines[1].split()))
        measurements = [list(map(float, line.split())) for line in lines[2:]]
        return frequencies, z_coordinate, measurements

def read_areas_from_file(file_path):
    with open(file_path, 'r') as file:
        lines = file.readlines()
        areas = []
        for line in lines:
            values = line.split()
            if len(values) == 5:
                r_min, z_min, r_max, z_max, conductivity = map(float, values)
                areas.append({
                    'r': (r_min, r_max),
                    'z': (z_min, z_max),
                    'conductivity': conductivity
                })
        return areas

# Функция для построения графика
def draw_phase_differences_plot_for_iteration(frequencies, z_coordinate, measurements, iteration_number):
    for i, measurement in enumerate(measurements):
        plt.plot(measurement, z_coordinate, 'o-', label=f'Frequency {frequencies[i]} MHz')
    plt.xlabel('Phase differences')
    plt.ylabel('Z')
    plt.title(f'Phase differences on iteration {iteration_number}')
    plt.ylim(-3.5, -2.5)
    plt.legend()
    plt.show()

def draw_areas_plot_for_iteration(areas, iteration):
    # Создаем фигуру и оси
    fig, ax = plt.subplots()

    # Генерируем данные для подобластей
    for i, subregion in enumerate(areas):
        r = np.linspace(subregion['r'][0], subregion['r'][1], 300)
        z = np.linspace(subregion['z'][0], subregion['z'][1], 300)
        R, Z = np.meshgrid(r, z)
        conductivity = np.full((300, 300), subregion['conductivity'])
        ax.pcolormesh(R, Z, conductivity, shading='auto', cmap='gist_rainbow', vmin=0.01, vmax=1)

        # Отрисовываем прямоугольники для областей
        width = subregion['r'][1] - subregion['r'][0]
        height = subregion['z'][1] - subregion['z'][0]
        rect = Rectangle((subregion['r'][0], subregion['z'][0]), width, height, edgecolor='black', facecolor='none')
        ax.add_patch(rect)

        # Убираем отрисовку значения проводимости для первой подобласти
        if i > 0:
            center_r = (subregion['r'][0] + subregion['r'][1]) / 2
            center_z = (subregion['z'][0] + subregion['z'][1]) / 2
            ax.text(center_r, center_z, f"{subregion['conductivity']:.4}", color='black', ha='center', va='center')

    # Настройки графика
    ax.set_xlabel('R')
    ax.set_ylabel('Z')
    ax.set_title(f'Areas on iteration {iteration}')
    ax.set_aspect('auto', adjustable='box')
    ax.set_xlim(1e-4, 3)
    ax.set_ylim(-6, 0)

    # Добавляем цветовую шкалу
    sm = plt.cm.ScalarMappable(cmap='gist_rainbow', norm=plt.Normalize(vmin=0.01, vmax=1))
    sm._A = []
    cbar = plt.colorbar(sm, ax=ax, label='Conductivity')

    # Отображаем график
    plt.show()

def draw_plot_for_true_values(frequencies, z_coordinate, measurements):
    for i, measurement in enumerate(measurements):
        plt.plot(measurement, z_coordinate, 'o-', label=f'Frequency {frequencies[i]} MHz')
    plt.xlabel('Phase differences')
    plt.ylabel('Z')
    plt.title(f'Phase differences true')
    plt.ylim(-3.5, -2.5)
    plt.legend()
    plt.show()

def draw_areas_plot_for_true_values(areas):
    # Создаем фигуру и оси
    fig, ax = plt.subplots()

    # Генерируем данные для подобластей
    for i, subregion in enumerate(areas):
        r = np.linspace(subregion['r'][0], subregion['r'][1], 300)
        z = np.linspace(subregion['z'][0], subregion['z'][1], 300)
        R, Z = np.meshgrid(r, z)
        conductivity = np.full((300, 300), subregion['conductivity'])
        ax.pcolormesh(R, Z, conductivity, shading='auto', cmap='gist_rainbow', vmin=0.01, vmax=1)

        # Отрисовываем прямоугольники для областей
        width = subregion['r'][1] - subregion['r'][0]
        height = subregion['z'][1] - subregion['z'][0]
        rect = Rectangle((subregion['r'][0], subregion['z'][0]), width, height, edgecolor='black', facecolor='none')
        ax.add_patch(rect)

        # Убираем отрисовку значения проводимости для первой подобласти
        if i > 0:
            center_r = (subregion['r'][0] + subregion['r'][1]) / 2
            center_z = (subregion['z'][0] + subregion['z'][1]) / 2
            ax.text(center_r, center_z, f"{subregion['conductivity']:.4}", color='black', ha='center', va='center')

    # Настройки графика
    ax.set_xlabel('R')
    ax.set_ylabel('Z')
    ax.set_title(f'Areas true')
    ax.set_aspect('auto', adjustable='box')
    ax.set_xlim(1e-4, 3)
    ax.set_ylim(-6, 0)

    # Добавляем цветовую шкалу
    sm = plt.cm.ScalarMappable(cmap='gist_rainbow', norm=plt.Normalize(vmin=0.01, vmax=1))
    sm._A = []
    cbar = plt.colorbar(sm, ax=ax, label='Conductivity')

    # Отображаем график
    plt.show()

# Директория, откуда нужно считать файлы
directory = "..\\InverseProblem\\Results\\5sigmas\\"

# Обработка каждого файла в директории
for file_name in os.listdir(directory):
    match = re.search(r'iteration (\d+)', file_name)
    if file_name.endswith('phase differences.txt'):
        # Извлечение номера итерации из названия файла
        file_path = os.path.join(directory, file_name)
        frequencies, z_coordinate, measurements = read_phase_differences_from_file(file_path)
        if match:
            iteration_number = int(match.group(1))
            draw_phase_differences_plot_for_iteration(frequencies, z_coordinate, measurements, iteration_number)
        elif file_name == 'true phase differences.txt':
            draw_plot_for_true_values(frequencies, z_coordinate, measurements)
    elif file_name.endswith('areas.txt'):
        file_path = os.path.join(directory, file_name)
        areas = read_areas_from_file(file_path)
        if match:
            iteration_number = int(match.group(1))
            draw_areas_plot_for_iteration(areas, iteration_number)
        elif file_name == 'true areas.txt':
            draw_areas_plot_for_true_values(areas)