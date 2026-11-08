// Front mínimo de MedProtect. Por ahora solo pregunta a la API si está viva y si llega a la base.
// Sin librerías ni recursos externos: la CSP del nginx solo permite archivos de este mismo sitio.
(function () {
  'use strict';

  async function revisar(ruta, elemento, bien) {
    const nodo = document.getElementById(elemento);
    try {
      const r = await fetch(ruta, { cache: 'no-store' });
      nodo.textContent = r.ok ? bien : `no responde (${r.status})`;
      nodo.className = r.ok ? 'ok' : 'mal';
    } catch (e) {
      nodo.textContent = 'sin conexión';
      nodo.className = 'mal';
    }
  }

  revisar('/api/health', 'estado-api', 'viva');
  revisar('/api/health/ready', 'estado-db', 'conectada');
})();
