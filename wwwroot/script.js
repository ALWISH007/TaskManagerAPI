
//Old code below
/*

// This is a MOCK version. It will only work in the browser's memory.
// It simulates what we will do later with a real API.

let tasks = []; // Our in-browser "database"

// DOM Elements
const taskForm = document.getElementById('taskForm');
const taskListDiv = document.getElementById('taskList');


// Event Listener for form submission
taskForm.addEventListener('submit', function (event) {
    event.preventDefault(); // Prevent page reload

    const titleInput = document.getElementById('title');
    const descriptionInput = document.getElementById('description');

    const title = titleInput.value.trim();
    const description = descriptionInput.value.trim();

    if (title) {
        addTask(title, description);
        // Reset the form
        taskForm.reset();
        titleInput.focus();
    }
});

// Initialize the page
renderTasks();

// Function to display tasks
function renderTasks() {
    if (tasks.length === 0) {
        taskListDiv.innerHTML = '<p>No tasks yet. Add one above!</p>';
        return;
    }

    taskListDiv.innerHTML = ''; // Clear the "Loading..." message and the <p> tag

    tasks.forEach(task => {
        const taskElement = document.createElement('div');
        taskElement.className = `task-item ${task.isCompleted ? 'task-completed' : ''}`;
        taskElement.innerHTML = `
            <h3>${task.title}</h3>
            <p>${task.description}</p>
            <button onclick="toggleTask(${task.id})">${task.isCompleted ? 'Mark Incomplete' : 'Mark Complete'}</button>
            <button onclick="deleteTask(${task.id})">Delete</button>
        `;
        taskListDiv.appendChild(taskElement);
    });
}

// Mock functions that will later be replaced with fetch() calls
function addTask(title, description) {
    const newTask = {
        id: Date.now(), // Simple unique ID
        title,
        description,
        isCompleted: false
    };
    tasks.push(newTask);
    renderTasks();
}

function toggleTask(id) {
    const task = tasks.find(t => t.id === id);
    if (task) {
        task.isCompleted = !task.isCompleted;
        renderTasks();
    }
}

function deleteTask(id) {
    tasks = tasks.filter(t => t.id !== id);
    renderTasks();
}

*/

//New code below

// Configuration - USE YOUR API's URL AND PORT HERE!
const API_BASE_URL = 'http://localhost:5000'; // <<< CHANGE THIS to your API's URL (the one from `dotnet run`)

// DOM Elements
const taskForm = document.getElementById('taskForm');
const taskListDiv = document.getElementById('taskList');

// Function to call the API and get all tasks
async function loadTasks() {
    try {
        showLoadingMessage();
        const response = await fetch(`${API_BASE_URL}/api/taskitems`);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const tasks = await response.json();
        renderTasks(tasks);
    } catch (error) {
        console.error('Failed to load tasks:', error);
        taskListDiv.innerHTML = '<p class="error">Failed to load tasks. Is the API running?</p>';
    }
}

// Function to display tasks
function renderTasks(tasks) {
    if (tasks.length === 0) {
        taskListDiv.innerHTML = '<p>No tasks yet. Add one above!</p>';
        return;
    }

    taskListDiv.innerHTML = ''; // Clear previous content

    tasks.forEach(task => {
        const taskElement = document.createElement('div');
        taskElement.className = `task-item ${task.isCompleted ? 'task-completed' : ''}`;
        taskElement.innerHTML = `
            <h3>${task.title}</h3>
            <p>${task.description || '(No description)'}</p>
            <div class="task-actions">
                <button onclick="toggleTask(${task.id}, ${!task.isCompleted})">
                    ${task.isCompleted ? 'Mark Incomplete' : 'Mark Complete'}
                </button>
                <button class="delete-btn" onclick="deleteTask(${task.id})">Delete</button>
            </div>
        `;
        taskListDiv.appendChild(taskElement);
    });
}

function showLoadingMessage() {
    taskListDiv.innerHTML = '<p>Loading tasks...</p>';
}

// Function to add a new task via the API
async function addTask(title, description) {
    try {
        const response = await fetch(`${API_BASE_URL}/api/taskitems`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                title: title,
                description: description,
                isCompleted: false
            })
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.title || 'Failed to create task');
        }

        // If successful, reload the task list
        loadTasks();
        return true;
    } catch (error) {
        console.error('Error adding task:', error);
        alert(`Error adding task: ${error.message}`);
        return false;
    }
}

// Function to update a task (toggle complete/incomplete)
async function toggleTask(id, newStatus) {
    try {
        // First, get the current task to update all its properties
        const getResponse = await fetch(`${API_BASE_URL}/api/taskitems/${id}`);
        if (!getResponse.ok) {
            throw new Error('Failed to fetch task for update');
        }

        const currentTask = await getResponse.json();

        // Now update the task with the new completion status
        const updateResponse = await fetch(`${API_BASE_URL}/api/taskitems/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                id: id,
                title: currentTask.title,
                description: currentTask.description,
                isCompleted: newStatus
            })
        });

        if (!updateResponse.ok) {
            throw new Error('Failed to update task');
        }

        // Reload the task list to reflect the change
        loadTasks();
    } catch (error) {
        console.error('Error toggling task:', error);
        alert('Error updating task');
    }
}

// Function to delete a task
async function deleteTask(id) {
    if (!confirm('Are you sure you want to delete this task?')) {
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/api/taskitems/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('Failed to delete task');
        }

        // Reload the task list
        loadTasks();
    } catch (error) {
        console.error('Error deleting task:', error);
        alert('Error deleting task');
    }
}

// Event Listener for form submission
taskForm.addEventListener('submit', async function (event) {
    event.preventDefault(); // Prevent page reload

    const titleInput = document.getElementById('title');
    const descriptionInput = document.getElementById('description');

    const title = titleInput.value.trim();
    const description = descriptionInput.value.trim();

    if (title) {
        const wasSuccessful = await addTask(title, description);
        if (wasSuccessful) {
            // Reset the form only if the API call was successful
            taskForm.reset();
            titleInput.focus();
        }
    } else {
        alert('Title is required.');
    }
});

// Initialize the page by loading tasks
loadTasks();