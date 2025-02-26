const apiBaseUrl = 'https://localhost:7252/api/tasks';
const token = localStorage.getItem('jwtToken');
if (!token) window.location.href = 'login.html';

let tasksList = [];

document.addEventListener('DOMContentLoaded', () => {
  fetchTasks();
  setInterval(fetchTasks, 60000); // Обновление задач каждую минуту
});

// Подключение к SignalR-хабу
const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5009/notificationHub", { withCredentials: true }) // Убедитесь, что URL корректен
  .build();

// Запуск подключения к SignalR
connection.start().catch(err => console.error("SignalR error: " + err.toString()));

// Обработка уведомлений через SignalR
connection.on("DeadlineNotification", function (data) {
  alert(`Внимание! Задача "${data.taskTitle}" заканчивается через ${data.minutesRemaining} минут(ы).`);
});

async function fetchTasks() {
  try {
    const response = await fetch(apiBaseUrl, {
      method: 'GET',
      headers: { 
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
      }
    });
    if (response.ok) {
      tasksList = await response.json();
      renderTasks(tasksList);
    } else {
      showMessage('Ошибка при получении задач', 'red');
    }
  } catch (error) {
    console.error(error);
    showMessage('Ошибка запроса', 'red');
  }
}

function renderTasks(tasks) {
  const tbody = document.querySelector('#tasksTable tbody');
  tbody.innerHTML = '';
  tasks.forEach(task => {
    const row = document.createElement('tr');
    row.innerHTML = `
      <td>${task.title}</td>
      <td>${task.description}</td>
      <td>${new Date(task.deadline).toLocaleString()}</td>
      <td>${getStatusText(task.status)}</td>
      <td>
        <button class="action-btn" onclick="editTask(${task.id})">Редактировать</button>
        <button class="action-btn" onclick="deleteTask(${task.id})">Удалить</button>
      </td>
    `;
    tbody.appendChild(row);
  });
}

function getStatusText(status) {
  return ['New', 'In Progress', 'Completed'][status] || 'Unknown';
}

document.getElementById('taskForm').addEventListener('submit', async (e) => {
  e.preventDefault();
  const taskId = document.getElementById('taskId') ? document.getElementById('taskId').value : '';
  const taskData = {
    title: document.getElementById('title').value,
    description: document.getElementById('description').value,
    deadline: document.getElementById('deadline').value,
    status: parseInt(document.getElementById('status').value),
    user: { id: 0, email: "string", passwordHash: "string" }
  };

  const method = taskId ? 'PUT' : 'POST';
  const url = taskId ? `${apiBaseUrl}/${taskId}` : apiBaseUrl;

  try {
    const response = await fetch(url, {
      method,
      headers: {
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(taskData)
    });
    if (response.ok) {
      showMessage('Задача сохранена', 'green');
      resetForm();
      fetchTasks();
    } else {
      const errText = await response.text();
      showMessage('Ошибка: ' + errText, 'red');
    }
  } catch (error) {
    console.error(error);
    showMessage('Ошибка запроса', 'red');
  }
});

function resetForm() {
  document.getElementById('taskForm').reset();
  document.getElementById('taskId').value = '';
  document.getElementById('formTitle').textContent = 'Добавить задачу';
  document.getElementById('cancelEdit').style.display = 'none';
}

document.getElementById('cancelEdit').addEventListener('click', resetForm);

function showMessage(message, color) {
  const messageDiv = document.getElementById('message');
  messageDiv.textContent = message;
  messageDiv.className = `alert ${color}`;
  setTimeout(() => { messageDiv.textContent = ''; }, 5000);
}

function editTask(id) {
  const task = tasksList.find(t => t.id === id);
  if (task) {
    document.getElementById('taskId').value = task.id;
    document.getElementById('title').value = task.title;
    document.getElementById('description').value = task.description;
    document.getElementById('deadline').value = task.deadline.substring(0, 16); // Формат YYYY-MM-DDTHH:mm
    document.getElementById('status').value = task.status;
    document.getElementById('formTitle').textContent = 'Редактировать задачу';
    document.getElementById('cancelEdit').style.display = 'inline-block';
  }
}

function deleteTask(id) {
  if (confirm('Вы уверены, что хотите удалить эту задачу?')) {
    fetch(`${apiBaseUrl}/${id}`, {
      method: 'DELETE',
      headers: { 'Authorization': 'Bearer ' + token }
    }).then(response => {
      if (response.ok) {
        showMessage('Задача удалена', 'green');
        fetchTasks();
      } else {
        showMessage('Ошибка удаления', 'red');
      }
    }).catch(error => {
      console.error(error);
      showMessage('Ошибка запроса', 'red');
    });
  }
}
