import json as json
import os
import random
from datetime import datetime
from itertools import chain
from tkinter import *
from tkinter import filedialog
from tkinter import ttk

import matplotlib.cm as cm
import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import seaborn as sns
## Gui Code
from matplotlib import animation

window = Tk()
window.title('Chemotaxis analytical tool')
window.geometry('400x300')
window.resizable(width=False, height=False)
window.configure(bg='white')

sns.set(rc={'figure.figsize': (18, 12)})
sns.set_context("notebook", font_scale=1.4)

# Data setup
# Opening JSON file
data_path = ''
world_width = 20
world_height = 20
data_len = 0
data = {}
The_cell = {}
directory = ''

now = datetime.now()
time_stamp = now.strftime("%d-%m-%Y-%H:%M:%S")


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
    for i in range(iteration_length):
        iteration = iterations[i]
        x = iteration['x']
        z = iteration['z']
        ap = iteration['ap']
        bp = iteration['bp']
        yp = iteration['yp']
        m = iteration['m']
        l = iteration['l']
        x_list.append(x)
        z_list.append(z)
        ap_list.append(ap)
        bp_list.append(bp)
        yp_list.append(yp)
        m_list.append(m)
        l_list.append(l)

    # The indexes: x= 0  ,z=1    ,ap=2   ,bp=3   ,yp=4  ,m=5   ,l=6   ,time=7
    parsed_data = (x_list, z_list, ap_list, bp_list, yp_list, m_list, l_list, time_list, iteration_length)
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


# Method to plot a path between 2 points

# The parameters should be changed to "cell" when the data exporting is working correctly
def path_plotter():
    cell_data = cell_parser(The_cell)
    fst_list = cell_data[0]
    snd_list = cell_data[1]
    with plt.rc_context({"figure.figsize": (12, 8)}):
        plt.plot(fst_list, snd_list, linewidth=2)
        plt.plot(0, 0, 'r*', markersize=9, label='Ligand source')
        plt.plot(fst_list[0], snd_list[0], 'go', markersize=9, label='Start point')
        plt.plot(fst_list[-1], snd_list[-1], 'ro', markersize=9, label='End point')
        rectangle = plt.Rectangle((-world_width / 2, -world_height / 2), world_width, world_height, fc='white',
                                  ec='black')
        plt.gca().add_patch(rectangle)
        plt.legend()
        plt.title('Path of a single cell (world view)')
        plt.axis('off')
    # plt.show()
    status_label.config(text='Plotting single cell path')
    plt.savefig(directory + '/cell_path.png')


# path_plotter()


# Method to plot a path between 2 points

# The parameters should be changed to "cell" when the data exporting is working correctly
def zoomed_path_plotter():
    cell_data = cell_parser(The_cell)
    fst_list = cell_data[0]
    snd_list = cell_data[1]
    with plt.rc_context({"figure.figsize": (12, 8)}):
        plt.plot(fst_list, snd_list, linewidth=3)
        plt.plot(0, 0, 'r*', markersize=15, label='Ligand source')
        plt.plot(fst_list[0], snd_list[0], 'go', markersize=12, label='Start point')
        plt.plot(fst_list[-1], snd_list[-1], 'ro', markersize=12, label='End point')

        plt.legend()
        plt.title('Path of a single cell (Zoomed in view) ')
        plt.axis('off')

    # plt.show()
    status_label.config(text='Plotting zoomed out cell path')
    plt.savefig(directory + '/zoomed_cell_path.png')


# path_plotter()

'''  
  cell_data = cell_parser(The_cell)
  fst_list = cell_data[0]
  snd_list = cell_data[1]
  with plt.rc_context({"figure.figsize":(12,8)}):
     fig, ax = plt.subplots(nrows=2, ncols=2)
     ax[0,0].plot(fst_list, snd_list,linewidth=2)
     ax[0,0].plot(0, 0, 'r*', markersize=9, label = 'Origo')
     ax[0,0].plot(fst_list[0], snd_list[0], 'go', markersize=9, label = 'Start point')
     ax[0,0].plot(fst_list[-1], snd_list[-1], 'ro', markersize=9,label = 'End point')
     ax[0,0].add_patch(plt.Rectangle((-world_width/2,-world_height/2), world_width, world_height,fc = 'white',ec ='black'))
     #ax[0,0].gca().add_patch(rectangle)   
     ax[0,0].legend()
     ax[0,0].title('Path of a single cell')
     plt.axis('off')
  plt.show()
  '''

1  # The parameter should be changed to "cell"


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

    # plt.show()
    status_label.config(text='plotting protein concentrations')
    plt.savefig(directory + '/concentrations.png')


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

    # plt.show()
    status_label.config(text='Plotting heatmap')
    plt.savefig(directory + '/heat_map_start_end.png')


# heatmap_plotter(data)

# creating a blank window
# for the animation
'''
cell_data = cell_parser(The_cell)
xs = cell_data[0]
zs = cell_data[1]
Writer = animation.writers['ffmpeg']
writer = Writer(fps=60, metadata=dict(artist='Me'), bitrate=1800)

fig = plt.figure() 
axis = plt.axes(xlim =(-world_width/2, world_height/2),
                ylim =(-world_width/2, world_height/2))
axis.grid('off') 

line, = axis.plot([], [], lw = 4) 

# what will our line dataset
# contain?
def init(): 
    line.set_data([], []) 
    return line, 

# initializing empty values
# for x and y co-ordinates
xdata, ydata = [], [] 

# animation function 
def animate(i): 
    # t is a parameter which varies
    # with the frame number
    t = 0.1 * i 

    # x, y values to be plotted 
    x = xs[i] 
    y = zs[i] 

    # appending values to the previously 
    # empty x and y data holders 
    xdata.append(x) 
    ydata.append(y) 
    line.set_data(xdata, ydata) 

    return line,

# calling the animation function     
anim = animation.FuncAnimation(fig, animate, init_func = init, 
                               frames = 1000, interval = 100, blit = True) 

# saves the animation in our desktop
anim.save('path.mp4', writer = writer)'''

'''
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation as animation


# create a figure with two subplots
fig, ax = plt.subplots(nrows=3, ncols=3)

# intialize two line objects (one in each axes)
line1, = ax1.plot([], [], lw=2)
line2, = ax2.plot([], [], lw=2, color='r')
line = [line1, line2]

# the same axes initalizations as before (just now we do it for both of them)

ax1.set_ylim(-world_width/2, world_height/2)
ax1.set_xlim(-world_width/2, world_height/2)
#ax.grid()

#ax[1,0].set_ylim(-world_width/2, world_height/2)
#ax[1,0].set_xlim(-world_width/2, world_height/2)
ax2.grid()


# initialize the data arrays 
x1data, y1data,x2data, y2data = [], [], [],[]
def run(i):
    # update the data
    x1 = xs[i] 
    y1 = zs[i]
    x2 = xs[i] 
    y2 = zs[i] 


    x1data.append(x1) 
    y1data.append(y1)

    x2data.append(x2)
    y2data.append(y2)





    # axis limits checking. Same as before, just for both axes
    #for ax in [ax1, ax2]:
       # xmin, xmax = ax.get_xlim()
      #  if t >= xmax:
        #    ax.set_xlim(xmin, 2*xmax)
         #   ax.figure.canvas.draw()

    # update the data of both line objects
    line[0].set_data(x1data, y1data)
    line[1].set_data(x2data, y2data)

    return line

ani2 = animation.FuncAnimation(fig, run, frames= 1000, blit=True, interval=100,
    repeat=False)
ani2.save('path2.mp4', writer = writer)
#plt.show()'''

'''

cell_data = cell_parser(The_cell)

x_list = cell_data[0][13:-1]
z_list = cell_data[1][13:-1]
ap_list = cell_data[2][13:-1]
bp_list = cell_data[3][13:-1]
yp_list = cell_data[4][13:-1]
m_list = cell_data[5][13:-1]
l_list = cell_data[6][13:-1]
time_list = cell_data[7][13:-1]
iteration_length = cell_data[8]

Writer = animation.writers['ffmpeg']
writer = Writer(fps=60, metadata=dict(artist='Me'), bitrate=1800)
x_max= 2* np.amax(x_list,0)
x_min = 2* np.amin(x_list,0)
z_max= 2* np.amax(z_list,0)
z_min= 2* np.amin(z_list,0)
ap_max = 1.3* np.amax(ap_list,0)
bp_max =  1.3* np.amax(bp_list,0)
yp_max =  1.3* np.amax(yp_list,0)
m_max =  1.3* np.amax(m_list,0)
l_max =  1.3* np.amax(l_list,0)

# create a figure with two subplots
fig, ((ax1, ax2), (ax3, ax4),(ax5,ax6)) = plt.subplots(nrows=3, ncols=2)

# intialize two line objects (one in each axes)
line1, = ax1.plot([], [], lw=2)
line2, = ax2.plot([], [], lw=2, color='r')
line3, = ax3.plot([], [], lw=2)
line4, = ax4.plot([], [], lw=2)
line5, = ax5.plot([], [], lw=2)
line6, = ax6.plot([], [], lw=2)
line = [line1, line2,line3, line4,line5, line6]

# the same axes initalizations as before (just now we do it for both of them)


ax1.set_xlim(x_min, x_max)
ax1.set_ylim(z_min,z_max)

ax1.grid(False)
ax1.axis(False)
#ax1.plot(0, 0, 'r*', markersize=10, label = 'Ligand source')



ax2.set_ylim(0, ap_max)
ax2.set_xlim(0, iteration_length)

ax2.set_ylim(0, ap_max)
ax2.set_xlim(0, iteration_length)

ax3.set_ylim(0, bp_max)
ax3.set_xlim(0, iteration_length)

ax4.set_ylim(0, yp_max)
ax4.set_xlim(0, iteration_length)

ax5.set_ylim(0, m_max)
ax5.set_xlim(0, iteration_length)

ax6.set_ylim(0, l_max)
ax6.set_xlim(0, iteration_length)

labels =  {ax1: 'cell path', ax2: 'CheA_P',ax3: 'CheB_P', ax4: 'CheY_P',ax5: 'M', ax6: 'Ligand'}
for key in labels:
  key.title.set_text(labels[key])



# initialize the data arrays 
x1data, y1data,x2data, x3data, x4data, x5data, x6data, timedata = [], [], [],[],[], [], [],[]
def run(i):
    # update the data
    x1 = x_list[i]
    x2 = ap_list[i]
    x3 = bp_list[i]
    x4 = yp_list[i]
    x5 = m_list[i]
    x6 = l_list[i]

    y1 = z_list[i]
    time = time_list[i] 


    x1data.append(x1) 
    y1data.append(y1)

    x2data.append(x2)
    x3data.append(x3) 
    x4data.append(x4)
    x5data.append(x5) 
    x6data.append(x6)
    timedata.append(time)





    # axis limits checking. Same as before, just for both axes
    #for ax in [ax1, ax2]:
       # xmin, xmax = ax.get_xlim()
      #  if t >= xmax:
        #    ax.set_xlim(xmin, 2*xmax)
         #   ax.figure.canvas.draw()

    # update the data of both line objects
    # inverted x and ys
    line[0].set_data(x1data, y1data)
    line[1].set_data(timedata, x2data)
    line[2].set_data(timedata, x3data)
    line[3].set_data(timedata, x4data)
    line[4].set_data(timedata, x5data)
    line[5].set_data(timedata, x6data)


    return line

ani2 = animation.FuncAnimation(fig, run, frames= 984, blit=True, interval=100,
    repeat=False)
ani2.save('/content/drive/MyDrive/Kandidatarbete Grupp 55/python scripts for data visualization/animation.mp4', writer = writer)
#plt.show()
'''

Writer = animation.writers['ffmpeg']
writer = Writer(fps=60, metadata=dict(artist='Me'), bitrate=1800)
fig, (ax1, ax2) = plt.subplots(nrows=2, ncols=1)
'''
cell_data = cell_parser(The_cell)

x_list = cell_data[0][13:-1]
z_list = cell_data[1][13:-1]
ap_list = cell_data[2][13:-1]
bp_list = cell_data[3][13:-1]
yp_list = cell_data[4][13:-1]
m_list = cell_data[5][13:-1]
l_list = cell_data[6][13:-1]
time_list = cell_data[7][13:-1]
iteration_length = cell_data[8]

x_max = 2 * np.amax(x_list, 0)
x_min = 2 * np.amin(x_list, 0)
z_max = 2 * np.amax(z_list, 0)
z_min = 2 * np.amin(z_list, 0)

l_max = 1.3 * np.amax(l_list, 0)

line1, = ax1.plot([], [], lw=2)
line2, = ax2.plot([], [], lw=2, color='r')

line = [line1, line2]

# the same axes initalizations as before (just now we do it for both of them)

ax1.set_xlim(x_min, x_max)
ax1.set_ylim(z_min, z_max)

ax1.grid(False)
ax1.axis(False)
# ax1.plot(0, 0, 'r*', markersize=10, label = 'Ligand source')

ax2.set_ylim(0, l_max)
ax2.set_xlim(0, iteration_length)

# initialize the data arrays
x1data, y1data, x2data, x3data, x4data, x5data, x6data, timedata = [], [], [], [], [], [], [], []


def run(i):
    # update the data
    x1 = x_list[i]
    x6 = l_list[i]
    y1 = z_list[i]
    time = time_list[i]

    x1data.append(x1)
    y1data.append(y1)
    x6data.append(x6)
    timedata.append(time)
    line[0].set_data(x1data, y1data)
    line[1].set_data(timedata, x6data)

    return line


def save_animation():
    status_label.configure(text='Working on the animation')
    ani2 = animation.FuncAnimation(fig, run, frames=984, blit=True, interval=100, repeat=False)
    ani2.save('animation.mp4', writer=writer)

'''


def do_the_job():
    path_plotter()
    zoomed_path_plotter()
    protein_concentration_plotter()
    heatmap_plotter()
    # save_animation()
    visualize_button.configure(state='disabled')



def browseFiles():
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
                        command=browseFiles)
button_explore.pack(pady=5)

visualize_button = Button(window, text='Visualize the data', command=do_the_job, bg='white')
visualize_button.pack(pady=5)
visualize_button.configure(state='disabled')

status_label = Label(window, text='')
status_label.place(x=87, y=240)
window.mainloop()


def main():
    print("Hello World!")


if __name__ == "__main__":
    main()
