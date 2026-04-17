<template>
  <footer class="footer">
    <!-- Partículas flotantes -->
    <div class="particles-footer">
      <div class="particle" v-for="n in 15" :key="n" :style="{ 
        left: `${Math.random() * 100}%`, 
        animationDelay: `${Math.random() * 5}s`,
        animationDuration: `${8 + Math.random() * 4}s`
      }"></div>
    </div>

    <!-- Onda decorativa superior con gradiente -->
    <div class="footer-wave">
      <svg viewBox="0 0 1200 120" preserveAspectRatio="none">
        <defs>
          <linearGradient id="waveGradient" x1="0%" y1="0%" x2="100%" y2="0%">
            <stop offset="0%" style="stop-color:#667eea;stop-opacity:0.1" />
            <stop offset="50%" style="stop-color:#764ba2;stop-opacity:0.1" />
            <stop offset="100%" style="stop-color:#667eea;stop-opacity:0.1" />
          </linearGradient>
        </defs>
        <path d="M0,40 C300,100 600,20 900,60 C1050,80 1150,40 1200,60 L1200,120 L0,120 Z" 
              class="wave-path-1"></path>
        <path d="M0,60 C250,20 550,80 800,50 C1000,30 1100,70 1200,50 L1200,120 L0,120 Z" 
              class="wave-path-2"></path>
      </svg>
    </div>

    <div class="footer-container">
      <!-- Contenido principal en grid -->
      <div class="footer-grid">
        <!-- Columna 1: Branding -->
        <div class="footer-column brand-column">
          <div class="brand-section">
            <div class="logo-wrapper">
              <div class="logo-glow-ring"></div>
              <img src="@/assets/imgs/Logo.png" alt="Logo" class="footer-logo" />
            </div>
            <h3 class="brand-name">The Training Hub</h3>
            <p class="brand-tagline">🎮 Level Up Your Fitness Journey</p>
            <p class="brand-description">
              La plataforma gamificada definitiva para transformar tus entrenamientos en una aventura épica.
            </p>
          </div>

          <!-- Stats badge -->
          <div class="stats-badge">
            <div class="stat-item">
              <span class="stat-icon">👥</span>
              <div class="stat-info">
                <span class="stat-value">10K+</span>
                <span class="stat-label">Usuarios</span>
              </div>
            </div>
            <div class="stat-item">
              <span class="stat-icon">🏆</span>
              <div class="stat-info">
                <span class="stat-value">50K+</span>
                <span class="stat-label">Retos</span>
              </div>
            </div>
            <div class="stat-item">
              <span class="stat-icon">⚡</span>
              <div class="stat-info">
                <span class="stat-value">100K+</span>
                <span class="stat-label">Check-ins</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Columna 2: Quick Links -->
        <div class="footer-column links-column">
          <h4 class="column-title">
            <span class="title-icon">🔗</span>
            Enlaces Rápidos
          </h4>
          <ul class="footer-links">
            <li><a href="#" class="footer-link">
              <span class="link-icon">🏠</span>
              <span class="link-text">Inicio</span>
            </a></li>
            <li><a href="#" class="footer-link">
              <span class="link-icon">🎯</span>
              <span class="link-text">Retos</span>
            </a></li>
            <li><a href="#" class="footer-link">
              <span class="link-icon">👥</span>
              <span class="link-text">Salas</span>
            </a></li>
            <li><a href="#" class="footer-link">
              <span class="link-icon">📊</span>
              <span class="link-text">Estadísticas</span>
            </a></li>
            <li><a href="#" class="footer-link">
              <span class="link-icon">🏅</span>
              <span class="link-text">Logros</span>
            </a></li>
          </ul>
        </div>

        <!-- Columna 3: Recursos -->
        <div class="footer-column resources-column">
          <h4 class="column-title">
            <span class="title-icon">📚</span>
            Recursos
          </h4>
          <ul class="footer-links">
            <li><a href="#" class="footer-link">
              <span class="link-icon">❓</span>
              <span class="link-text">Centro de Ayuda</span>
            </a></li>
            <li><a href="#" class="footer-link">
              <span class="link-icon">📖</span>
              <span class="link-text">Guía de Usuario</span>
            </a></li>
            <li><a href="#" class="footer-link">
              <span class="link-icon">💡</span>
              <span class="link-text">Tips de Entrenamiento</span>
            </a></li>
            <li><a href="#" class="footer-link">
              <span class="link-icon">🎓</span>
              <span class="link-text">Tutoriales</span>
            </a></li>
            <li><a href="#" class="footer-link">
              <span class="link-icon">📰</span>
              <span class="link-text">Blog</span>
            </a></li>
          </ul>
        </div>

        <!-- Columna 4: Comunidad -->
        <div class="footer-column community-column">
          <h4 class="column-title">
            <span class="title-icon">🌟</span>
            Comunidad
          </h4>
          
          <p class="social-intro">Únete a nuestra comunidad y conecta con miles de entrenadores</p>
          
          <div class="social-grid">
            <a href="#" class="social-card twitter" aria-label="Twitter">
              <div class="social-bg"></div>
              <img src="@/assets/imgs/twitter.png" alt="Twitter" class="social-icon" />
              <span class="social-name">Twitter</span>
            </a>
            <a href="#" class="social-card facebook" aria-label="Facebook">
              <div class="social-bg"></div>
              <img src="@/assets/imgs/facebook.png" alt="Facebook" class="social-icon" />
              <span class="social-name">Facebook</span>
            </a>
            <a href="#" class="social-card instagram" aria-label="Instagram">
              <div class="social-bg"></div>
              <img src="@/assets/imgs/instagram.png" alt="Instagram" class="social-icon" />
              <span class="social-name">Instagram</span>
            </a>
            <a href="#" class="social-card tiktok" aria-label="TikTok">
              <div class="social-bg"></div>
              <img src="@/assets/imgs/tik-tok.png" alt="TikTok" class="social-icon" />
              <span class="social-name">TikTok</span>
            </a>
          </div>

          <!-- Newsletter -->
          <div class="newsletter-box">
            <div class="newsletter-icon">📧</div>
            <p class="newsletter-text">Recibe tips y actualizaciones</p>
            <button class="newsletter-btn">Suscribirse</button>
          </div>
        </div>
      </div>

      <!-- Separador decorativo -->
      <div class="footer-divider">
        <div class="divider-line"></div>
        <div class="divider-icon">💪</div>
        <div class="divider-line"></div>
      </div>

      <!-- Footer Bottom -->
      <div class="footer-bottom">
        <div class="bottom-left">
          <p class="copyright">© 2025 The Training Hub. All rights reserved.</p>
          <p class="made-with">
            Made with <span class="heart">❤️</span> by fitness enthusiasts
          </p>
        </div>

        <div class="bottom-center">
          <div class="badge-container">
            <div class="achievement-badge">
              <span class="badge-emoji">🎮</span>
              <span class="badge-text">Gamified Platform</span>
            </div>
            <div class="achievement-badge">
              <span class="badge-emoji">🔒</span>
              <span class="badge-text">Secure & Safe</span>
            </div>
          </div>
        </div>

        <div class="bottom-right">
          <div class="legal-links">
            <a href="#" class="legal-link">Privacy Policy</a>
            <span class="dot">•</span>
            <a href="#" class="legal-link">Terms of Service</a>
            <span class="dot">•</span>
            <a href="#" class="legal-link">Cookies</a>
            <span class="dot">•</span>
            <a href="#" class="legal-link">Contact</a>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.footer {
  position: relative;
  background: linear-gradient(135deg, #0f0c29 0%, #302b63 50%, #24243e 100%);
  color: white;
  padding-top: 0;
  overflow: hidden;
  margin-top: 5rem;
  font-family: 'Patrick Hand', cursive;
}

/* Partículas flotantes */
.particles-footer {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 1;
}

.particle {
  position: absolute;
  width: 3px;
  height: 3px;
  background: #ffc107;
  border-radius: 50%;
  opacity: 0;
  animation: float-up linear infinite;
  box-shadow: 0 0 8px #ffc107;
}

@keyframes float-up {
  0% {
    transform: translateY(0) scale(0);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 0.7;
  }
  100% {
    transform: translateY(-800px) scale(1);
    opacity: 0;
  }
}

/* Ondas decorativas */
.footer-wave {
  position: absolute;
  top: -1px;
  left: 0;
  width: 100%;
  height: 120px;
  overflow: hidden;
  line-height: 0;
  z-index: 2;
}

.footer-wave svg {
  position: relative;
  display: block;
  width: calc(100% + 1.3px);
  height: 120px;
}

.wave-path-1 {
  fill: rgba(255, 255, 255, 0.05);
  animation: waveMove1 8s ease-in-out infinite;
}

.wave-path-2 {
  fill: rgba(255, 255, 255, 0.03);
  animation: waveMove2 12s ease-in-out infinite;
}

@keyframes waveMove1 {
  0%, 100% {
    d: path("M0,40 C300,100 600,20 900,60 C1050,80 1150,40 1200,60 L1200,120 L0,120 Z");
  }
  50% {
    d: path("M0,60 C300,30 600,80 900,40 C1050,60 1150,70 1200,50 L1200,120 L0,120 Z");
  }
}

@keyframes waveMove2 {
  0%, 100% {
    d: path("M0,60 C250,20 550,80 800,50 C1000,30 1100,70 1200,50 L1200,120 L0,120 Z");
  }
  50% {
    d: path("M0,50 C250,90 550,30 800,70 C1000,50 1100,40 1200,60 L1200,120 L0,120 Z");
  }
}

/* Container principal */
.footer-container {
  position: relative;
  z-index: 3;
  max-width: 1400px;
  margin: 0 auto;
  padding: 120px 3rem 2rem;
}

/* Grid del footer */
.footer-grid {
  display: grid;
  grid-template-columns: 1.5fr 1fr 1fr 1.2fr;
  gap: 3rem;
  margin-bottom: 3rem;
}

/* Columnas */
.footer-column {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

/* Columna de Branding */
.brand-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.logo-wrapper {
  position: relative;
  width: 100px;
  height: 100px;
  margin-bottom: 0.5rem;
}

.logo-glow-ring {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 120px;
  height: 120px;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(255, 193, 7, 0.3), transparent);
  animation: ringPulse 3s ease-in-out infinite;
}

@keyframes ringPulse {
  0%, 100% {
    transform: translate(-50%, -50%) scale(1);
    opacity: 0.5;
  }
  50% {
    transform: translate(-50%, -50%) scale(1.2);
    opacity: 0.8;
  }
}

.footer-logo {
  width: 100%;
  height: 100%;
  border-radius: 50%;
  border: 3px solid rgba(255, 193, 7, 0.5);
  box-shadow: 0 8px 32px rgba(255, 193, 7, 0.3);
  object-fit: cover;
  transition: transform 0.3s ease;
}

.footer-logo:hover {
  transform: rotate(10deg) scale(1.05);
}

.brand-name {
  font-size: 1.8rem;
  font-weight: bold;
  color: #ffc107;
  margin: 0;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
}

.brand-tagline {
  font-size: 1.1rem;
  color: #ccc;
  margin: 0;
  font-weight: 600;
}

.brand-description {
  font-size: 0.95rem;
  color: #aaa;
  line-height: 1.6;
  margin: 0;
}

/* Stats Badge */
.stats-badge {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.8rem;
  margin-top: 1rem;
}

.stat-item {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 193, 7, 0.3);
  border-radius: 12px;
  padding: 0.8rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.3rem;
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

.stat-item:hover {
  transform: translateY(-5px);
  border-color: rgba(255, 193, 7, 0.6);
  background: rgba(255, 255, 255, 0.08);
}

.stat-icon {
  font-size: 1.5rem;
}

.stat-info {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.stat-value {
  font-size: 1.2rem;
  font-weight: bold;
  color: #ffc107;
}

.stat-label {
  font-size: 0.75rem;
  color: #ccc;
}

/* Títulos de columnas */
.column-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 1.3rem;
  color: #ffc107;
  margin: 0 0 1rem 0;
  font-weight: bold;
}

.title-icon {
  font-size: 1.5rem;
}

/* Links */
.footer-links {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 0.8rem;
}

.footer-link {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  color: #ccc;
  text-decoration: none;
  font-size: 0.95rem;
  padding: 0.5rem;
  border-radius: 8px;
  transition: all 0.3s ease;
}

.footer-link:hover {
  color: #ffc107;
  background: rgba(255, 193, 7, 0.1);
  transform: translateX(8px);
}

.link-icon {
  font-size: 1.1rem;
}

.link-text {
  font-weight: 500;
}

/* Comunidad */
.social-intro {
  font-size: 0.9rem;
  color: #aaa;
  line-height: 1.5;
  margin: 0;
}

.social-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 0.8rem;
}

.social-card {
  position: relative;
  background: rgba(255, 255, 255, 0.05);
  border: 2px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  padding: 1rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  text-decoration: none;
  overflow: hidden;
  transition: all 0.3s ease;
}

.social-card:hover {
  transform: translateY(-5px);
  border-color: rgba(255, 255, 255, 0.3);
}

.social-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.social-card.twitter:hover .social-bg {
  background: linear-gradient(135deg, #1DA1F2, #0d8bd9);
  opacity: 0.2;
}

.social-card.facebook:hover .social-bg {
  background: linear-gradient(135deg, #4267B2, #365899);
  opacity: 0.2;
}

.social-card.instagram:hover .social-bg {
  background: linear-gradient(135deg, #E1306C, #C13584, #833AB4);
  opacity: 0.2;
}

.social-card.tiktok:hover .social-bg {
  background: linear-gradient(135deg, #000000, #fe2c55);
  opacity: 0.2;
}

.social-icon {
  width: 32px;
  height: 32px;
  filter: brightness(0) invert(1);
  position: relative;
  z-index: 2;
  transition: transform 0.3s ease;
}

.social-card:hover .social-icon {
  transform: scale(1.1);
}

.social-name {
  font-size: 0.85rem;
  color: white;
  font-weight: 600;
  position: relative;
  z-index: 2;
}

/* Newsletter */
.newsletter-box {
  background: linear-gradient(135deg, rgba(102, 126, 234, 0.2), rgba(118, 75, 162, 0.2));
  border: 2px solid rgba(102, 126, 234, 0.4);
  border-radius: 16px;
  padding: 1.2rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.8rem;
  text-align: center;
  backdrop-filter: blur(10px);
}

.newsletter-icon {
  font-size: 2rem;
}

.newsletter-text {
  font-size: 0.9rem;
  color: #ccc;
  margin: 0;
  font-weight: 600;
}

.newsletter-btn {
  background: linear-gradient(135deg, #667eea, #764ba2);
  color: white;
  border: none;
  border-radius: 8px;
  padding: 0.6rem 1.5rem;
  font-size: 0.9rem;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s ease;
  font-family: 'Patrick Hand', cursive;
}

.newsletter-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

/* Separador decorativo */
.footer-divider {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin: 2rem 0;
}

.divider-line {
  flex: 1;
  height: 2px;
  background: linear-gradient(90deg, transparent, rgba(255, 193, 7, 0.5), transparent);
}

.divider-icon {
  font-size: 1.8rem;
  filter: drop-shadow(0 0 8px rgba(255, 193, 7, 0.5));
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.1);
  }
}

/* Footer Bottom */
.footer-bottom {
  display: grid;
  grid-template-columns: 1fr auto 1fr;
  align-items: center;
  gap: 2rem;
  padding: 2rem 0 1rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.bottom-left {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.copyright {
  font-size: 0.9rem;
  color: #aaa;
  margin: 0;
}

.made-with {
  font-size: 0.85rem;
  color: #999;
  margin: 0;
}

.heart {
  color: #ff6b6b;
  display: inline-block;
  animation: heartbeat 1.5s ease-in-out infinite;
}

@keyframes heartbeat {
  0%, 100% {
    transform: scale(1);
  }
  10%, 30% {
    transform: scale(1.2);
  }
  20%, 40% {
    transform: scale(1);
  }
}

.bottom-center {
  display: flex;
  justify-content: center;
}

.badge-container {
  display: flex;
  gap: 1rem;
}

.achievement-badge {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 193, 7, 0.3);
  border-radius: 20px;
  padding: 0.5rem 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  backdrop-filter: blur(10px);
}

.badge-emoji {
  font-size: 1.1rem;
}

.badge-text {
  font-size: 0.8rem;
  color: #ffc107;
  font-weight: 600;
}

.bottom-right {
  display: flex;
  justify-content: flex-end;
}

.legal-links {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  flex-wrap: wrap;
}

.legal-link {
  color: #aaa;
  text-decoration: none;
  font-size: 0.85rem;
  transition: color 0.3s ease;
}

.legal-link:hover {
  color: #ffc107;
}

.dot {
  color: #666;
  font-size: 0.7rem;
}

/* Responsive Design */
@media (max-width: 1200px) {
  .footer-grid {
    grid-template-columns: 1.5fr 1fr 1fr;
    gap: 2rem;
  }

  .community-column {
    grid-column: 1 / -1;
  }

  .social-grid {
    grid-template-columns: repeat(4, 1fr);
  }
}

@media (max-width: 768px) {
  .footer-container {
    padding: 100px 2rem 2rem;
  }

  .footer-grid {
    grid-template-columns: 1fr;
    gap: 2.5rem;
  }

  .stats-badge {
    grid-template-columns: repeat(3, 1fr);
  }

  .social-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .footer-bottom {
    grid-template-columns: 1fr;
    gap: 1.5rem;
    text-align: center;
  }

  .bottom-left,
  .bottom-right {
    justify-content: center;
  }

  .legal-links {
    justify-content: center;
  }

  .badge-container {
    flex-direction: column;
    width: 100%;
  }

  .achievement-badge {
    justify-content: center;
  }
}

@media (max-width: 480px) {
  .footer-container {
    padding: 80px 1.5rem 1.5rem;
  }

  .footer-grid {
    gap: 2rem;
  }

  .logo-wrapper {
    width: 80px;
    height: 80px;
  }

  .brand-name {
    font-size: 1.5rem;
  }

  .brand-tagline {
    font-size: 1rem;
  }

  .brand-description {
    font-size: 0.9rem;
  }

  .stats-badge {
    grid-template-columns: 1fr;
  }

  .social-grid {
    grid-template-columns: 1fr;
  }

  .column-title {
    font-size: 1.2rem;
  }

  .footer-link {
    font-size: 0.9rem;
  }

  .legal-links {
    flex-direction: column;
    gap: 0.5rem;
  }

  .dot {
    display: none;
  }
}
</style>
