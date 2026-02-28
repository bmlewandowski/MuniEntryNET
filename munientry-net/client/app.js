async function check() {
  try {
    const res = await fetch('http://localhost:5000/api/health');
    const data = await res.json();
    document.getElementById('status').textContent = 'API: ' + JSON.stringify(data);
  } catch (e) {
    document.getElementById('status').textContent = 'API error: ' + e;
  }
}
check();
