import json as json
import os
import random
from itertools import chain
from tkinter import *
from tkinter import filedialog
from tkinter import messagebox

import matplotlib.cm as cm
import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import seaborn as sns

## Gui Code

window = Tk()
window.title('Chemotaxis analytical tool')
window.geometry('400x300')
window.resizable(width=False, height=False)
window.configure(bg='white')

sns.set(rc={'figure.figsize': (12, 10)})
sns.set_context("notebook", font_scale=1.4)

# Data setup

data_path = ''
world_width = 26
world_height = 26
data_len = 0
data = {}
The_cell = {}
directory = ''


def createFolder(folder_name):
    try:
        if not os.path.exists(folder_name):
            os.makedirs(folder_name)
    except OSError:
        print('Error: Creating directory. ' + folder_name)


def open_file(path):
    f = open(path, )

    the_data = json.load(f)
    global data_len
    data_len = len(the_data) - 1
    global data
    data = the_data


# Generate a new random cell for the visualization
def cell_randomizer(data):
    data_len = len(data)
    random_index = random.randint(0, data_len - 1)
    cell = data[random_index]
    return cell


# The idea of this function is that given a specific data,
# take the generated random cell and parse needed info. Afterwards,
# the parsed data is returned as a dict
def cell_parser(cell):
    iterations = cell['Iterations']
    iteration_length = len(iterations)
    time_list = np.arange(0, iteration_length, 1)

    x_list = []
    z_list = []
    ap_list = []
    bp_list = []
    yp_list = []
    m_list = []
    l_list = []
    life_list = []
    death_list = []
    for i in range(iteration_length):
        iteration = iterations[i]
        x = iteration['x']
        z = iteration['z']
        ap = iteration['ap']
        bp = iteration['bp']
        yp = iteration['yp']
        m = iteration['m']
        l = iteration['l']
        life = iteration['life']
        death = iteration['death']
        x_list.append(x)
        z_list.append(z)
        ap_list.append(ap)
        bp_list.append(bp)
        yp_list.append(yp)
        m_list.append(m)
        l_list.append(l)
        life_list.append(life)
        death_list.append(death)

    # The indexes: x= 0  ,z=1    ,ap=2   ,bp=3   ,yp=4  ,m=5   ,l=6   ,time=7
    parsed_data = (
        x_list, z_list, ap_list, bp_list, yp_list, m_list, l_list, time_list, iteration_length, life_list, death_list)
    return parsed_data


# The purpose of this fuction is to get all occurencies for a specific info
# type for all cells ( like getting all x values in the json.file )
# info_index parameter is the index of the info needed to be extracted
# the indexes are these for the dictionary in parse_cell function
def All_info_parser(info_index):
    info_list = []

    for i in range(data_len):
        cell = data[i]
        cell_data = cell_parser(cell)
        info = cell_data[info_index]
        info_list.append(info)

    flatten_list = list(chain.from_iterable(info_list))
    return flatten_list


# Function meant to return start and end x,z coords for all the
# cells in the data. This will come in handy in meshgrid plotting
def start_end_point_parser():
    xs = All_info_parser(0)
    zs = All_info_parser(1)
    iterations = The_cell['Iterations']
    iteration_length = len(iterations)

    start_xs = []
    start_zs = []
    for i in range(data_len):
        x = xs[i * iteration_length]
        z = zs[i * iteration_length]
        start_xs.append(x)
        start_zs.append(z)

    end_xs = []
    end_zs = []
    for j in range(data_len):
        x = xs[(j * iteration_length) + iteration_length - 1]
        z = zs[(j * iteration_length) + iteration_length - 1]
        end_xs.append(x)
        end_zs.append(z)
    return start_xs, start_zs, end_xs, end_zs


def path_plotter():
    cell_data = cell_parser(The_cell)
    fst_list = cell_data[0]
    snd_list = cell_data[1]

    plt.plot(fst_list, snd_list, linewidth=2)
    plt.plot(0, 0, 'r*', markersize=9, label='Ligand source')
    plt.plot(fst_list[0], snd_list[0], 'go', markersize=9, label='Start point')
    plt.plot(fst_list[-1], snd_list[-1], 'ro', markersize=9, label='End point')
    rectangle = plt.Rectangle((-world_width / 2, -world_height / 2), world_width, world_height, fc='white', ec='black')
    plt.gca().add_patch(rectangle)
    plt.legend()
    plt.title('Path of a single cell (world view)')
    plt.axis('off')
    # plt.show()
    plt.savefig(directory + '/cell_path.png')
    plt.clf()


def zoomed_path_plotter():
    cell_data = cell_parser(The_cell)
    fst_list = cell_data[0]
    snd_list = cell_data[1]

    plt.plot(fst_list, snd_list, linewidth=3)
    plt.plot(0, 0, 'r*', markersize=15, label='Ligand source')
    plt.plot(fst_list[0], snd_list[0], 'go', markersize=12, label='Start point')
    plt.plot(fst_list[-1], snd_list[-1], 'ro', markersize=12, label='End point')
    plt.legend()
    plt.title('Path of a single cell (Zoomed in view) ')
    plt.axis('off')
    plt.savefig(directory + '/zoomed_cell_path.png')
    plt.clf()


def protein_concentration_plotter():
    cell_data = cell_parser(The_cell)

    ap_list = cell_data[2]
    bp_list = cell_data[3]
    yp_list = cell_data[4]
    m_list = cell_data[5]
    l_list = cell_data[6]
    time_list = cell_data[7]

    # ap_max = np.amax(ap_list, 0)
    # bp_max = np.amax(bp_list, 0)
    # yp_max = np.amax(yp_list, 0)
    # m_max = np.amax(m_list, 0)
    # l_max = np.amax(l_list, 0)

    fig, ax = plt.subplots(nrows=3, ncols=3)
    plt.plot(figsize=(20, 9))
    plt.title('Protein and Ligand converntration for a given cell')

    ax[0, 0].plot(time_list[15:-1], ap_list[15:-1], 'r')
    ax[0, 0].title.set_text('CheA_P')

    ax[0, 2].plot(time_list[15:-1], bp_list[15:-1], 'b')
    ax[0, 2].title.set_text('CheB_P')

    ax[2, 0].plot(time_list[15:-1], yp_list[15:-1], 'g')
    ax[2, 0].title.set_text('CheY_P')

    ax[1, 1].plot(time_list[10:-1], l_list[10:-1], 'k')
    ax[1, 1].title.set_text('Ligand')

    ax[2, 2].plot(time_list[10:-1], m_list[10:-1], 'k')
    ax[2, 2].title.set_text('M')

    ax[0, 1].axis('off')
    ax[1, 0].axis('off')
    ax[1, 2].axis('off')
    ax[2, 1].axis('off')

    plt.savefig(directory + '/concentrations.png')
    plt.clf()


# protein_concentration_plotter()


def heatmap_plotter():
    start_xs, start_zs, end_xs, end_zs = start_end_point_parser()
    fig = plt.figure(figsize=(16, 20))
    # ===============
    #  First subplot
    # ===============
    # set up the axes for the first plot
    ax = fig.add_subplot(2, 1, 1, projection='3d')

    X = start_xs
    Y = start_zs
    X, Y = np.meshgrid(X, Y)
    R = np.sqrt(X ** 2 + Y ** 2)
    Z = np.sin(R)
    dimension = X.shape[0]
    reshaping_dimension = np.square(dimension)
    x = X.reshape(reshaping_dimension)
    y = Y.reshape(reshaping_dimension)
    z = Z.reshape(reshaping_dimension)
    df = pd.DataFrame({'x': x, 'y': y, 'z': z}, index=range(len(x)))
    m = plt.cm.ScalarMappable(cmap=cm.coolwarm)
    m.set_array(Z)
    c = fig.colorbar(m, ax=ax)
    # ax.axis('off')
    ax.set_xticks([])
    ax.set_yticks([])
    ax.set_zticks([])
    c.set_label('Change of density through area')
    ax.set_title('Initial positions for the cell population')
    ax.plot_trisurf(df.x, df.y, df.z, cmap=cm.jet, linewidth=0.2)

    # ===============
    # Second subplot
    # ===============
    # set up the axes for the second plot

    ax = fig.add_subplot(2, 1, 2, projection='3d')
    X1 = end_xs
    Y1 = end_zs
    X1, Y1 = np.meshgrid(X1, Y1)
    R1 = np.sqrt(X1 ** 2 + Y1 ** 2)
    Z1 = np.sin(R1)
    dimension = X1.shape[0]
    reshaping_dimension = np.square(dimension)
    x1 = X1.reshape(reshaping_dimension)
    y1 = Y1.reshape(reshaping_dimension)
    z1 = Z1.reshape(reshaping_dimension)
    df = pd.DataFrame({'x': x1, 'y': y1, 'z': z1}, index=range(len(x1)))
    m = plt.cm.ScalarMappable(cmap=cm.coolwarm)
    m.set_array(Z)
    c = fig.colorbar(m, ax=ax)
    c.set_label('Change of density through area')

    ax.set_xticks([])
    ax.set_yticks([])
    ax.set_zticks([])
    ax.set_title('Final positions for the cell population')
    ax.plot_trisurf(df.x, df.y, df.z, cmap=cm.jet, linewidth=0.2)

    plt.savefig(directory + '/heat_map_start_end.png')
    plt.clf()


def population_change():
    cell_data = cell_parser(The_cell)
    plt.plot(cell_data[7], data[-1])
    plt.title('Population change throughout the simulation')
    plt.ylabel('Number of cells')
    plt.xlabel('iteration')
    plt.savefig(directory + '/population_change')
    plt.clf()


def life_death_analysis():
    cell_data = cell_parser(The_cell)

    fig, (ax1, ax2) = plt.subplots(nrows=2, ncols=1)
    plt.plot(figsize=(16, 12))
    plt.title('Dath and life changes against ligand concentration for a cell')

    ax1.plot(cell_data[7], cell_data[10], 'r')
    ax1.title.set_text('Death')

    ax2.plot(cell_data[7], cell_data[9], 'b')
    ax2.title.set_text('life')

    plt.savefig(directory + '/death_and_life')
    plt.clf()


def average_ligand_concentration():
    cell_data = cell_parser(The_cell)
    l_sum_list = []
    l_sum = 0
    cell_index = 0
    for i in cell_data[7]:
        while cell_index < data_len:
            l_sum += data[cell_index]['Iterations'][i]['l']
            # print(l_sum)
            cell_index += 1

        l_sum_list.append(l_sum / 333)
        l_sum = 0
        cell_index = 0

    plt.plot(cell_data[7], l_sum_list)
    plt.ylabel('Average concentration for the cell population')
    plt.xlabel('Iteration')
    plt.savefig(directory + '/average_ligand_concentration')
    plt.clf()


def do_the_job():
    path_plotter()
    zoomed_path_plotter()
    protein_concentration_plotter()
    heatmap_plotter()
    population_change()
    life_death_analysis()
    average_ligand_concentration()
    visualize_button.configure(state='disabled')
    messagebox.showinfo(title='Success', message='The files has been saved to {}'.format(data_path))
    go_to_dir_button.configure(state='active')


#


def browse_files():
    global data_path
    global The_cell
    global directory
    filename = filedialog.askopenfilename(initialdir=os.getcwd(),
                                          title="Select a File",
                                          filetypes=(("JSON files",
                                                      "*.json*"),
                                                     ("all files",
                                                      "*.*")))
    if not filename:
        return
    else:
        name = os.path.basename(filename)
        label_file_explorer.configure(text="File Opened: " + name)
        data_path = filename
        open_file(data_path)
        The_cell = cell_randomizer(data)
        directory = 'Simulation_{}'.format(name)
        createFolder(directory)
        visualize_button.configure(state='active')


def go_to_dir():
    os.startfile(directory)


# GUI COMPONENTS
welcome_text = Label(window,
                     text='Welcome to our analytical tool \n Please choose a file',
                     bg='white')
welcome_text.pack(pady=3)

label_file_explorer = Label(window,
                            text="No file selected",
                            width=100, height=4,
                            fg="blue")
label_file_explorer.pack(pady=5)

button_explore = Button(window,
                        text="Browse Files",
                        command=browse_files)
button_explore.pack(pady=5)

visualize_button = Button(window, text='Visualize the data', command=do_the_job, bg='white')
visualize_button.pack(pady=20)
visualize_button.configure(state='disabled')

go_to_dir_button = Button(window, text='Go to target folder', command=go_to_dir, bg='white')
go_to_dir_button.pack(pady=20)
go_to_dir_button.configure(state='disabled')
window.mainloop()
