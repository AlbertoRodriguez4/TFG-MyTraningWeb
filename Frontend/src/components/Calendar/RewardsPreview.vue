<template>
  <div class="rewards-preview mt-8">
    <h3 class="text-h6 font-weight-bold mb-4 text-center">
      🎁 Recompensas al Completar
    </h3>
    <v-row>
      <v-col cols="4">
        <div class="reward-preview-card">
          <div class="reward-icon purple">
            <v-icon size="50" color="white">mdi-star</v-icon>
          </div>
          <div class="reward-amount">+150</div>
          <div class="reward-type">Experiencia</div>
        </div>
      </v-col>
      <v-col cols="4">
        <div class="reward-preview-card">
          <div class="reward-icon amber">
            <v-icon size="50" color="white">mdi-coin</v-icon>
          </div>
          <div class="reward-amount">+50</div>
          <div class="reward-type">Monedas</div>
        </div>
      </v-col>
      <v-col cols="4">
        <div class="reward-preview-card">
          <div class="reward-icon orange">
            <v-icon size="50" color="white">mdi-fire</v-icon>
          </div>
          <div class="reward-amount">+1</div>
          <div class="reward-type">Racha</div>
        </div>
      </v-col>
    </v-row>
  </div>
</template>

<script>
export default {
  name: 'RewardsPreview'
};
</script>

<style scoped>
.rewards-preview {
  background: linear-gradient(135deg, rgba(26, 10, 46, 0.6) 0%, rgba(15, 10, 26, 0.8) 100%);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 20px;
  padding: 2rem;
  backdrop-filter: blur(10px);
}

.reward-preview-card {
  text-align: center;
  background: rgba(26, 10, 46, 0.7);
  border-radius: 16px;
  padding: 2rem 1rem;
  border: 2px solid rgba(139, 92, 246, 0.3);
  transition: all 0.3s ease;
}

.reward-preview-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 12px 30px rgba(139, 92, 246, 0.3);
  border-color: rgba(167, 139, 250, 0.6);
}

.reward-icon {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1rem;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.3);
}

.reward-icon.purple {
  background: linear-gradient(135deg, #a78bfa 0%, #22d3ee 100%);
}

.reward-icon.amber {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
}

.reward-icon.orange {
  background: linear-gradient(135deg, #fb7185 0%, #f43f5e 100%);
}

.reward-amount {
  font-size: 2rem;
  font-weight: 900;
  color: #ffffff;
  margin-bottom: 0.5rem;
  text-shadow: 0 0 20px rgba(167, 139, 250, 0.3);
}

.reward-type {
  font-size: 0.95rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.7);
}

@media (max-width: 600px) {
  .rewards-preview {
    padding: 1rem;
  }
  
  .reward-preview-card {
    padding: 1rem 0.5rem;
  }
  
  .reward-icon {
    width: 50px;
    height: 50px;
  }
  
  .reward-icon .v-icon {
    font-size: 32px !important;
  }
  
  .reward-amount {
    font-size: 1.2rem;
  }
  
  .reward-type {
    font-size: 0.75rem;
  }
}
</style>