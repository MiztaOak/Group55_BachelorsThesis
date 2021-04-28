import json as json
import math
import os
import random
import threading
from idlelib.tooltip import Hovertip
from itertools import chain
from tkinter import filedialog, Button, Label, Tk, Menu, messagebox, IntVar, ttk

import matplotlib.pyplot as plt
import numpy as np
import seaborn as sns
## Gui Code
from matplotlib.lines import Line2D

window = Tk()
window.title('Chemotaxis analytical tool')
window.geometry('500x400')
window.resizable(width=False, height=False)
window.configure(bg='white')

sns.set(rc={'figure.figsize': (14, 10)})
# sns.set_context(font_scale=0.5)

# Data setup

data_path = ''
world_width = 26
world_height = 26
data_len = 0
data = {}
The_cell = {}
directory = ''
file_list = []

#####
secondary_data_path = ''
secondary_data_len = 0
secondary_data = {}
secondary_cell = {}


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


def open_secondary_file(path):
    f = open(path, )

    the_data = json.load(f)
    global secondary_data_len
    secondary_data_len = len(the_data) - 1
    global secondary_data
    secondary_data = the_data


# Generate a new random cell for the visualization
def cell_randomizer(data):
    random_index = random.randint(0, len(data) - 1)
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
    birth_date_list = []
    death_date_list = []
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
        birth_date = iteration['birth_date']
        death_date = iteration['death_date']
        x_list.append(x)
        z_list.append(z)
        ap_list.append(ap)
        bp_list.append(bp)
        yp_list.append(yp)
        m_list.append(m)
        l_list.append(l)
        life_list.append(life)
        death_list.append(death)
        birth_date_list.append(birth_date)
        death_date_list.append(death_date)

    # The indexes: x= 0  ,z=1    ,ap=2   ,bp=3   ,yp=4  ,m=5   ,l=6   ,time=7
    parsed_data = (
        x_list, z_list, ap_list, bp_list, yp_list, m_list, l_list, time_list, iteration_length, life_list, death_list,
        birth_date_list, death_date_list)
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
    birth_date = cell_data[11][0]
    has_died, death_date, cell_id = cell_obituary_notice(The_cell)
    print('BIRTH : ', birth_date)
    plt.plot(fst_list, snd_list, linewidth=2)
    plt.plot(0, 0, 'r*', markersize=9, label='Ligand source')
    plt.plot(fst_list[birth_date], snd_list[birth_date], 'go', markersize=9, label='Start point')
    plt.plot(fst_list[-1], snd_list[-1], 'ro', markersize=9, label='End point')
    rectangle = plt.Rectangle((-world_width / 2, -world_height / 2), world_width, world_height, fc='white',
                              ec='black')
    plt.gca().add_patch(rectangle)
    plt.legend()
    if has_died:
        caption = 'The cell was born at iteration {} and died at iteration {}'.format(birth_date, death_date)
        plt.plot(fst_list[death_date], snd_list[death_date])
        plt.title('Path of a single cell (world view) \n{}'.format(caption))

    else:
        plt.title('Path of a single cell (world view)')
    plt.axis('off')
    # plt.show()
    plt.savefig(directory + '/cell_path.png')
    plt.clf()


def zoomed_path_plotter():
    cell_data = cell_parser(The_cell)
    fst_list = cell_data[0]
    snd_list = cell_data[1]
    birth_date = cell_data[11][0]
    has_died, death_date, cell_id = cell_obituary_notice(The_cell)
    print('BIRTH : ', birth_date)
    plt.plot(fst_list, snd_list, linewidth=2)
    plt.plot(0, 0, 'r*', markersize=9, label='Ligand source')
    plt.plot(fst_list[birth_date], snd_list[birth_date], 'go', markersize=9, label='Start point')
    plt.plot(fst_list[-1], snd_list[-1], 'ro', markersize=9, label='End point')
    plt.legend()
    if has_died:
        caption = 'The cell was born at iteration {} and died at iteration {}'.format(birth_date, death_date)
        plt.plot(fst_list[death_date], snd_list[death_date])
        plt.title('Path of a single cell (Zoomed in view) \n{}'.format(caption))

    else:
        plt.title('Path of a single cell (Zoomed in view)')
    plt.axis('off')
    plt.savefig(directory + '/zoomed_cell_path.png')
    plt.clf()


def MSD_calc(cell):
    cell_data = cell_parser(cell)
    x_data = cell_data[0]
    z_data = cell_data[1]
    r = np.column_stack((x_data, z_data))
    shifts = np.arange(len(r))
    MSD_list = np.zeros(shifts.size)

    for i, shift in enumerate(shifts):
        square_dist = np.linalg.norm(r[i])
        MSD_list[i] = square_dist + MSD_list[i - 1]

    MSD_list = [i / len(MSD_list) for i in MSD_list]

    return MSD_list, cell_data[7]


def MSD_plotter():
    MSD_list, time = MSD_calc(The_cell)
    plt.plot(time, MSD_list)
    plt.title('Mean square displacement (MSD) for a random cell')
    plt.xlabel('iterations')
    plt.ylabel('MSD score')
    plt.savefig(directory + '/MSD.png')
    plt.clf()


def MSD_plotter_fitted():
    MSD_list, time = MSD_calc(The_cell)

    unfitted_line = np.linspace(1, time, 100)

    polynomial_fitter = np.poly1d(np.polyfit(time, MSD_list, 2))
    fitted_line = polynomial_fitter(unfitted_line)
    plt.plot(unfitted_line, fitted_line, 'b')
    plt.plot(time, MSD_list, 'r')

    colors = ['green', 'blue']
    lines = [Line2D([0], [0], color=c, linewidth=3, linestyle='-') for c in colors]
    labels = ['unfitted line', 'fitted line']
    legend = plt.legend(lines, labels, loc='lower center', shadow=True)

    # Put a nicer background color on the legend.
    legend.get_frame().set_facecolor('ivory')
    plt.title('Polynomial fitted mean square displacement for a random cell')
    plt.xlabel('iterations')
    plt.ylabel('MSD score')
    plt.savefig(directory + '/MSD_fitted.png')
    plt.clf()


def double_MSD_plotter():
    fst_MSD_list, fst_time = MSD_calc(The_cell)
    snd_MSD_list, snd_time = MSD_calc(secondary_cell)

    fig, (ax1, ax2) = plt.subplots(nrows=2, ncols=1)
    plt.plot(figsize=(16, 12))
    fig.suptitle('Mean Squared Displacement (MSD)', fontsize=15)
    fig.tight_layout(pad=3.0)

    ax1.plot(fst_time, fst_MSD_list)
    ax1.set_ylabel('MSD score')
    ax1.set_xlabel('iteration \n \n \n')
    ax1.title.set_text('First file')

    ax2.plot(snd_time, snd_MSD_list)
    ax2.set_ylabel('MSD score')
    ax2.set_xlabel('iteration')
    ax2.title.set_text('Second file')

    plt.savefig(directory + '/double_MSD')
    plt.clf()


def double_MSD_plotter_fitted():
    fst_MSD_list, fst_time = MSD_calc(The_cell)
    snd_MSD_list, snd_time = MSD_calc(secondary_cell)

    unfitted_line = np.linspace(1, fst_time, 100)
    polynomial_fitter = np.poly1d(np.polyfit(fst_time, fst_MSD_list, 2))
    fitted_line = polynomial_fitter(unfitted_line)

    unfitted_line2 = np.linspace(1, snd_time, 100)
    polynomial_fitter2 = np.poly1d(np.polyfit(snd_time, snd_MSD_list, 2))
    fitted_line2 = polynomial_fitter2(unfitted_line2)

    fig, (ax1, ax2) = plt.subplots(nrows=2, ncols=1)
    plt.plot(figsize=(16, 12))
    fig.suptitle('polynomial fitted Mean Squared Displacement (MSD)', fontsize=15)
    fig.tight_layout(pad=3.0)

    ax1.plot(unfitted_line, fitted_line, 'b.')
    ax1.plot(fst_time, fst_MSD_list, 'r-')
    ax1.set_ylabel('MSD score')
    ax1.set_xlabel('iteration')
    ax1.title.set_text('First file')

    ax2.plot(unfitted_line2, fitted_line2, 'b.')
    ax2.plot(snd_time, snd_MSD_list, 'r-')
    ax2.set_xlabel('iteration')
    ax2.set_ylabel('MSD score')
    ax2.title.set_text('Second file')

    plt.savefig(directory + '/double_MSD2')
    plt.clf()


def protein_concentration_plotter():
    cell_data = cell_parser(The_cell)
    has_died, death_date, cell_id = cell_obituary_notice(The_cell)

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

    plt.title('Protein and Ligand concentration for a given cell')

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

'''
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

'''


def population_change():
    cell_data = cell_parser(The_cell)
    plt.plot(cell_data[7], data[-1])
    plt.title('Population change throughout the simulation')
    plt.ylabel('Number of cells')
    plt.xlabel('iteration')
    plt.savefig(directory + '/population_change')
    plt.clf()


def double_population_change():
    cell_data = cell_parser(The_cell)
    secondary_cell_data = cell_parser(secondary_cell)

    fig, (ax1, ax2) = plt.subplots(nrows=2, ncols=1)
    plt.plot(figsize=(16, 12))
    fig.suptitle('Population change throughout the simulation', fontsize=15)
    fig.tight_layout(pad=3.0)

    ax1.plot(cell_data[7], data[-1])
    ax1.set_ylabel('Number of cells')
    ax1.set_xlabel('iteration \n \n \n')
    ax1.title.set_text('First file')

    ax2.plot(secondary_cell_data[7], secondary_data[-1])
    ax2.set_ylabel('Number of cells')
    ax2.set_xlabel('iteration')
    ax2.title.set_text('Second file')

    plt.savefig(directory + '/double_population_change')
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


def average_ligand_concentration_calc(cell, Data):
    cell_data = cell_parser(cell)
    iterations = cell_data[7]
    cell_count = Data[-1]
    l_sum_list = []
    l_sum = 0
    cell_index = 0
    for i in iterations:

        while cell_index < len(Data) - 1:
            cell = Data[cell_index]['Iterations'][i]
            if cell['birth_date'] > i or cell['death_date'] < i:
                cell_index += 1
            else:
                l_sum += cell['l']
                l_sum = l_sum
                cell_index += 1

        l_sum_list.append(l_sum / cell_count[i])
        l_sum = 0
        cell_index = 0
    return iterations, l_sum_list


def average_ligand_concentration_plotter():
    iterations, l_sum_list = average_ligand_concentration_calc(The_cell, data)

    plt.plot(iterations[3:], l_sum_list[3:])
    plt.ylabel('Average concentration')
    plt.xlabel('Iteration')
    plt.title('Cell population average ligand concentration')
    plt.savefig(directory + '/average_ligand_concentration')
    plt.clf()


def double_average_ligand_concentration_plotter():
    fst_iterations, fst_l_sum_list = average_ligand_concentration_calc(The_cell, data)
    snd_iterations, snd_l_sum_list = average_ligand_concentration_calc(secondary_cell, secondary_data)

    fig, (ax1, ax2) = plt.subplots(nrows=2, ncols=1)
    plt.plot(figsize=(16, 12))
    fig.suptitle('Population change throughout the simulation', fontsize=15)
    fig.tight_layout(pad=3.0)

    ax1.plot(fst_iterations[3:], fst_l_sum_list[3:])
    ax1.set_ylabel('Average concentration for the cell population')
    ax1.set_xlabel('iteration')
    ax1.title.set_text('First file')

    ax2.plot(snd_iterations[3:], snd_l_sum_list[3:])
    ax2.set_ylabel('Average concentration for the cell population')
    ax2.set_xlabel('iteration')
    ax2.title.set_text('Second file')

    plt.savefig(directory + '/double_average_ligand_concentration')
    plt.clf()


# return if cell has died and when
def cell_obituary_notice(cell):
    cell_data = cell_parser(cell)
    death_date_list = cell_data[12]
    if death_date_list[0] != 0 and death_date_list[0] != cell_data[8] + 1:
        print('Cell {} was murdered at iteration: {}'.format(cell['id'], death_date_list[0]))
        return True, death_date_list[0], cell['id']
    else:
        print('The cell is buzzin')
        return False, cell_data[8], 0


def cell_population_information():
    dead_cells = []
    new_born_cells = []

    for i in data[:-1]:
        for j in range(len(i['Iterations'])):
            if i['Iterations'][j]['death'] == 1:
                dead_cells.append(i['id'])
                break

    for i in data[:-1]:
        if int(i['Iterations'][0]['birth_date']) > 0:
            new_born_cells.append(i['id'])

    survived_cells = [x for x in range(len(data[:-1])) if x not in dead_cells]

    return new_born_cells, dead_cells, survived_cells


def average_distance_calc(d, cell):
    init_data = cell_parser(cell)
    iterations = init_data[7]
    avg_distance_iteration = []
    avg_distance_list = []
    cell_index = 0

    for i in iterations:
        while cell_index < len(d) - 1:
            x = d[cell_index]['Iterations'][i]['x']
            z = d[cell_index]['Iterations'][i]['z']
            dist = math.hypot(x - 0, z - 0)
            avg_distance_iteration.append(dist)
            cell_index += 1

        avg_dist = np.mean(avg_distance_iteration)
        avg_distance_list.append(avg_dist)
        cell_index = 0
        avg_distance_iteration.clear()

    return avg_distance_list, init_data[7]


def average_distance_plotter():
    average_distance, time = average_distance_calc(data, The_cell)
    plt.plot(time, average_distance)
    plt.xlabel('iteration')
    plt.ylabel('distance in unity distance unit')
    plt.title('Average distance for the cell population from origin')
    plt.savefig(directory + '/average_distance')
    plt.clf()


def double_average_distance_plotter():
    fst_average_distance, fst_time = average_distance_calc(data, The_cell)
    snd_average_distance, snd_time = average_distance_calc(secondary_data, secondary_cell)

    fig, (ax1, ax2) = plt.subplots(nrows=2, ncols=1)
    plt.plot(figsize=(16, 12))
    fig.suptitle('Averga distance for the cell population from origin', fontsize=15)
    fig.tight_layout(pad=3.0)

    ax1.plot(fst_time, fst_average_distance)
    ax1.set_ylabel('Average distance for the cell population from origin')
    ax1.set_xlabel('iteration')
    ax1.title.set_text('First file')

    ax2.plot(snd_time, snd_average_distance)
    ax2.set_ylabel('Average distance for the cell population from origin')
    ax2.set_xlabel('iteration')
    ax2.title.set_text('Second file')

    plt.savefig(directory + '/double_average_distance')
    plt.clf()


'''
def pdf_generator():
    a, b, c = cell_population_information()
    pdf = FPDF(orientation='P', unit='pt', format='A4')
    pdf.set_auto_page_break(auto=True, margin=15)
    pdf.add_page()
    pdf.set_font("Times", "B", 24)
    pdf.cell(0, 80, "Purchase Receipt", 0, 1, "C")
    pdf.set_font("Times", "B", 14)
    pdf.cell(100, 25, "Payment Date:")
    pdf.set_font("Times", "", 12)
    pdf.cell(0, 25, "{}".format(a), 0, 1)
    pdf.cell(0, 5, "", 0, 1)
    pdf.set_font("Times", "B", 14)
    pdf.cell(100, 25, "Payment Total:")
    pdf.set_font("Times", "", 12)
    pdf.cell(0, 25, "${}".format(b), 0, 1)
    pdf.output('test.pdf')
'''


def populate_data_structures(filename):
    global data_path
    global The_cell
    global directory

    name = os.path.basename(filename)
    data_path = filename
    open_file(data_path)
    The_cell = cell_randomizer(data)
    if radio_choice.get() == 1:
        directory = 'Simulation_{}'.format(name)
    if radio_choice.get() == 2:
        directory = 'Batch analysis_{}'.format(name)
    if radio_choice.get() == 3:
        directory = 'Double file analysis_{}'.format(name)
    createFolder(directory)


def populate_secondary_data_structures(filename):
    global secondary_data_path
    global secondary_cell

    secondary_data_path = filename
    open_secondary_file(secondary_data_path)
    secondary_cell = cell_randomizer(secondary_data)


def browse_file():
    filename = filedialog.askopenfilename(initialdir=os.getcwd(),
                                          title="Select a File",
                                          filetypes=(("JSON files",
                                                      "*.json*"),
                                                     ("all files",
                                                      "*.*")))

    if not filename:
        return
    else:
        # print(filename)
        populate_data_structures(filename)
        name = os.path.basename(filename)
        label_file_explorer.configure(text="File Opened: " + name)
        visualize_button.configure(state='active')


def browse_fst_file():
    done_label.configure(text='')
    filename = filedialog.askopenfilename(initialdir=os.getcwd(),
                                          title="Select a File",
                                          filetypes=(("JSON files",
                                                      "*.json*"),
                                                     ("all files",
                                                      "*.*")))

    if not filename:
        return
    else:
        # print(filename)
        populate_data_structures(filename)
        name = os.path.basename(filename)
        fst_file_label.configure(text="File Opened: " + name)
        snd_button_explore.configure(state='active')


def browse_snd_file():
    filename = filedialog.askopenfilename(initialdir=os.getcwd(),
                                          title="Select a File",
                                          filetypes=(("JSON files",
                                                      "*.json*"),
                                                     ("all files",
                                                      "*.*")))

    if not filename:
        return
    else:
        # print(filename)
        populate_secondary_data_structures(filename)
        name = os.path.basename(filename)
        snd_file_label.configure(text="File Opened: " + name)
        visualize_button.configure(state='active')


def browse_folder():
    global data_path
    global The_cell
    global directory
    global file_list
    file_list.clear()
    folder_name = filedialog.askdirectory()
    if not folder_name:
        return
    else:
        label_file_explorer.configure(text="Folder Opened: " + os.path.basename(folder_name))
        for file in os.listdir(folder_name):
            if file.endswith(".json"):
                filename = os.path.join(folder_name, file)
                file_list.append(filename)
                print(len(file_list))
        visualize_button.configure(state='active')


def on_browse_click():
    done_label.configure(text='')
    if radio_choice.get() == 1:
        browse_file()
    if radio_choice.get() == 2:
        browse_folder()


def go_to_dir():
    os.startfile(directory)


def do_the_job():
    if radio_choice.get() == 1:

        path_plotter()
        zoomed_path_plotter()
        life_death_analysis()
        protein_concentration_plotter()
        MSD_plotter()
        MSD_plotter_fitted()
        average_ligand_concentration_plotter()
        population_change()

        average_distance_plotter()

        visualize_button.configure(state='disable')
        go_to_dir_button.configure(state='active')
        label_file_explorer.configure(text="No file selected")

    if radio_choice.get() == 2:
        for file in file_list:
            populate_data_structures(file)
            path_plotter()
            zoomed_path_plotter()
            life_death_analysis()
            protein_concentration_plotter()
            MSD_plotter()
            MSD_plotter_fitted()
            average_ligand_concentration_plotter()
            average_distance_plotter()
        go_to_dir_button.configure(state='active')
        visualize_button.configure(state='disable')
        label_file_explorer.configure(text="No file selected")

    if radio_choice.get() == 3:
        double_average_ligand_concentration_plotter()
        double_population_change()
        double_MSD_plotter()
        double_MSD_plotter_fitted()
        double_average_distance_plotter()

        go_to_dir_button.configure(state='active')
        fst_file_label.configure(text="No file selected")
        snd_file_label.configure(text="No file selected")
        snd_button_explore.configure(state='disable')
        visualize_button.configure(state='disable')
    pb.stop()
    done_label.config(text='DONE !')


def thread_starter():
    pb.start()
    done_label.configure(text='Working...')
    threading.Thread(target=do_the_job).start()


def alter_scene():
    fst_button_explore.place(x=100, y=175)
    snd_button_explore.place(x=300, y=175)
    fst_file_label.place(x=100, y=110)
    snd_file_label.place(x=300, y=110)
    button_explore.place_forget()
    label_file_explorer.place_forget()


def re_alter_scene():
    fst_button_explore.place_forget()
    snd_button_explore.place_forget()
    fst_file_label.place_forget()
    snd_file_label.place_forget()
    button_explore.place(x=195, y=175)
    label_file_explorer.place(x=195, y=110)


# bg = ImageTk.PhotoImage(PIL.Image.open("bg.png"))
# label1 = Label(window, image=bg)
# label1.place(x=0, y=0)
# GUI COMPONENTS
welcome_text = Label(window,
                     text='Welcome to our analytical tool \n Please choose a file',
                     bg='white')
welcome_text.place(x=170, y=10)
# welcome_text.pack(pady=(3, 50))

radio_choice = IntVar(None, 1)
R1 = ttk.Radiobutton(window, text="Single file analysis", style='Wild.TRadiobutton', variable=radio_choice, value=1,
                     command=re_alter_scene)
R1Tip = Hovertip(R1, 'Perform analysis on a single json file')
R1.place(x=13, y=60)

R2 = ttk.Radiobutton(window, text="Batch analysis", style='Wild.TRadiobutton', variable=radio_choice, value=2,
                     command=re_alter_scene)
R2Tip = Hovertip(R2, 'perform analysis on a batch with multiple json file')
R2.place(x=200, y=60)

R3 = ttk.Radiobutton(window, text="Double file analysis", style='Wild.TRadiobutton', variable=radio_choice, value=3,
                     command=alter_scene)
R3Tip = Hovertip(R3, 'perform analysis on two json files')
R3.place(x=363, y=60)

label_file_explorer = Label(window, text="No file selected", fg="blue", height=2, anchor="e")
label_file_explorer.place(x=195, y=110)

fst_file_label = Label(window, text="No file selected", fg="blue", height=2, anchor="e")

snd_file_label = Label(window, text="No file selected", fg="blue", height=2, anchor="e")

button_explore = Button(window, text="Browse Files", command=on_browse_click, width=15, height=1)
button_explore.place(x=195, y=175)
# button_explore.pack(pady=5)

fst_button_explore = Button(window, text="Browse first file", command=browse_fst_file, width=15, height=1)

snd_button_explore = Button(window, text="Browse second file", command=browse_snd_file, width=15, height=1)
snd_button_explore.configure(state='disable')

visualize_button = Button(window, text='Visualize the data', command=thread_starter, bg='white', width=15, height=1)
visualize_button.place(x=195, y=220)
visualize_button.configure(state='disabled')

go_to_dir_button = Button(window, text='Go to Output folder', command=go_to_dir, bg='white', width=15, height=1)
go_to_dir_button.place(x=195, y=270)
# go_to_dir_button.pack(pady=20)
go_to_dir_button.configure(state='disabled')


def to_help():
    messagebox.showinfo('info',
                        "This program was programmed by students in Chalmers university of technology."
                        "It is used to do analysis reports and plots of the data obtained from the Chemotaxis "
                        "simulation tool")


def to_How_to():
    messagebox.showinfo('info',
                        "Start with choosing the type of analysis you want to preform."
                        " \n For Single File Analysis choose one exported json file."
                        "\n For Batch Analysis choose a folder containing json files."
                        "\n For Double File Analysis choose two files to compare")


menubar = Menu(window)

help_menu = Menu(menubar, tearoff=0)
help_menu.add_command(label="How to use", command=to_How_to)
help_menu.add_command(label="About...", command=to_help)
help_menu.add_separator()
help_menu.add_command(label="Exit", command=window.quit)
menubar.add_cascade(label="Help", menu=help_menu)

window.config(menu=menubar)

s = ttk.Style()
s.configure('Wild.TRadiobutton', background='white', foreground='black')

pb = ttk.Progressbar(
    window,
    orient='horizontal',
    mode='indeterminate',

    length=120
)
pb.place(x=190, y=320)
pbTip = Hovertip(pb, 'Please be kind and restart the program \nif the progress bar runs for a long time')

done_label = Label(window, text="", fg="green", height=2, anchor="e", background='white')
done_label.config(font=("Arial", 14, 'bold'))
done_label.place(x=215, y=345)
window.mainloop()

'''
def cell_family_tree(cell):
    global relatives
    family = []
    cell_data = cell_parser(cell)
    birth_date = cell_data[11]

    for next_cell in data[:-1]:
        next_cell_data = cell_parser(next_cell)
        if birth_date != 0:
            if (cell_data[0][birth_date] == next_cell_data[0][birth_date]) and (
                    cell_data[1][birth_date] == next_cell_data[1][birth_date]):
                family.append(list(data).index(next_cell))
    relatives = list(dict.fromkeys(relatives))
    return relatives
'''
