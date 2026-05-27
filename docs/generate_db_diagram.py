#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
generate_db_diagram.py

Genera un diagrama ER de la base de datos de TheTrainingHub como imagen PNG
usando matplotlib (sin dependencia de Graphviz binario).

Salida: db_diagram.png en la raiz del proyecto.
"""

import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
from matplotlib.patches import FancyBboxPatch, FancyArrowPatch
import os

# Directorio de salida (raiz del proyecto)
OUTPUT_DIR = os.path.dirname(os.path.abspath(__file__))
OUTPUT_FILE = os.path.join(OUTPUT_DIR, "db_diagram.png")

# Definicion de tablas: nombre, columnas[(nombre, tipo, extra)]
TABLES = {
    "users": {
        "columns": [
            ("id", "int", "PK"),
            ("name", "string", ""),
            ("email", "string", ""),
            ("passwordhash", "string", ""),
            ("level", "int", "default 1"),
            ("strength", "int", "default 0"),
            ("endurance", "int", "default 0"),
            ("consistencystreak", "int", "default 0"),
            ("gold", "int", "default 0"),
            ("role", "string", ""),
            ("experience", "int", "default 0"),
            ("equippedStrengthId", "int?", "FK -> items"),
            ("equippedEnduranceId", "int?", "FK -> items"),
            ("avatarUrl", "string?", ""),
            ("isEmailVerified", "bool", "default false"),
        ],
        "pos": (0, 0),
        "color": "#1a1a2e",
        "accent": "#ffcc00",
    },
    "items": {
        "columns": [
            ("id", "int", "PK"),
            ("name", "string", ""),
            ("type", "string", "Strength | Endurance"),
            ("bonus", "int", ""),
            ("price", "int", ""),
            ("imageUrl", "string?", ""),
        ],
        "pos": (6, 4),
        "color": "#16213e",
        "accent": "#38bdf8",
    },
    "purchases": {
        "columns": [
            ("id", "int", "PK"),
            ("userid", "int", "FK -> users"),
            ("itemid", "int", "FK -> items"),
            ("purchasedate", "datetime", "UTC"),
        ],
        "pos": (3, 4),
        "color": "#16213e",
        "accent": "#34d399",
    },
    "plans": {
        "columns": [
            ("id", "int", "PK"),
            ("userid", "int", "FK -> users"),
            ("description", "string", ""),
        ],
        "pos": (-3, 4),
        "color": "#16213e",
        "accent": "#a78bfa",
    },
    "subscriptions": {
        "columns": [
            ("id", "int", "PK"),
            ("userid", "int", "FK -> users"),
            ("startdate", "datetime", "UTC"),
            ("enddate", "datetime", "UTC"),
            ("isactive", "bool", "default true"),
            ("planType", "string", 'default "Premium"'),
            ("monthlyPrice", "decimal", "default 10.00"),
        ],
        "pos": (-6, 0),
        "color": "#16213e",
        "accent": "#f472b6",
    },
    "rooms": {
        "columns": [
            ("id", "int", "PK"),
            ("name", "string", ""),
            ("minlevel", "int", "default 1"),
            ("minstats", "int", "default 0"),
            ("minconsistency", "int", "default 0"),
            ("description", "string", ""),
            ("date", "string", ""),
            ("localization", "string", ""),
            ("creatorRole", "string?", ""),
        ],
        "pos": (6, -4),
        "color": "#16213e",
        "accent": "#fbbf24",
    },
    "usersrooms": {
        "columns": [
            ("userid", "int", "FK -> users, PK"),
            ("roomid", "int", "FK -> rooms, PK"),
        ],
        "pos": (3, -4),
        "color": "#0f172a",
        "accent": "#fbbf24",
    },
    "tasks": {
        "columns": [
            ("id", "int", "PK"),
            ("name", "string", ""),
            ("description", "string", ""),
            ("difficulty", "int", ""),
            ("reward", "int", ""),
            ("iscompleted", "bool", "default false"),
            ("createdat", "datetime", ""),
            ("userId", "int", "FK -> users"),
            ("trainingfocus", "string", ""),
        ],
        "pos": (-3, -4),
        "color": "#16213e",
        "accent": "#94a3b8",
    },
    "emailverifications": {
        "columns": [
            ("id", "int", "PK"),
            ("userid", "int", "FK -> users"),
            ("code", "string", ""),
            ("createdat", "datetime", "UTC"),
            ("expiresat", "datetime", "UTC"),
            ("isused", "bool", "default false"),
        ],
        "pos": (-6, -4),
        "color": "#16213e",
        "accent": "#e2e8f0",
    },
    "notificationpreferences": {
        "columns": [
            ("id", "int", "PK"),
            ("userid", "int", "FK -> users, Unique"),
            ("inactivityEnabled", "bool", "default true"),
            ("inactivityDays", "int", "default 3"),
            ("roomsEnabled", "bool", "default true"),
            ("purchasesEnabled", "bool", "default true"),
            ("subscriptionExpiryEnabled", "bool", "default true"),
            ("createdat", "datetime", "UTC"),
            ("updatedat", "datetime", "UTC"),
        ],
        "pos": (-6, 4),
        "color": "#0f172a",
        "accent": "#cbd5e1",
    },
    "achievements": {
        "columns": [
            ("Id", "int", "PK"),
            ("Name", "string", ""),
            ("Description", "string", ""),
            ("IconUrl", "string", ""),
            ("Category", "string", ""),
            ("RequirementType", "string", ""),
            ("RequirementValue", "int", ""),
            ("RewardGold", "int", "default 0"),
            ("RewardXP", "int", "default 0"),
            ("RewardItemId", "string?", ""),
            ("IsSecret", "bool", "default false"),
        ],
        "pos": (9, 0),
        "color": "#16213e",
        "accent": "#fbbf24",
    },
    "userachievements": {
        "columns": [
            ("Id", "int", "PK"),
            ("UserId", "int", "FK -> users"),
            ("AchievementId", "int", "FK -> achievements"),
            ("UnlockedAt", "datetime", "UTC"),
            ("IsNew", "bool", "default true"),
        ],
        "pos": (9, -4),
        "color": "#0f172a",
        "accent": "#fbbf24",
    },
    "bodymetrics": {
        "columns": [
            ("Id", "int", "PK"),
            ("UserId", "int", "FK -> users"),
            ("Date", "datetime", "UTC"),
            ("Weight", "float?", "kg"),
            ("Height", "float?", "cm"),
            ("BodyFat", "float?", "%"),
            ("Chest", "float?", "cm"),
            ("Waist", "float?", "cm"),
            ("Arms", "float?", "cm"),
            ("Thighs", "float?", "cm"),
            ("ProgressPhotoUrl", "string?", ""),
            ("Notes", "string?", ""),
        ],
        "pos": (0, -8),
        "color": "#16213e",
        "accent": "#34d399",
    },
    "exercises": {
        "columns": [
            ("Id", "int", "PK"),
            ("Name", "string", ""),
            ("Description", "string", ""),
            ("MuscleGroup", "string", ""),
            ("Difficulty", "string", ""),
            ("ImageUrl", "string?", ""),
            ("VideoUrl", "string?", ""),
            ("Equipment", "string", ""),
            ("Instructions", "string?", ""),
            ("CommonMistakes", "string?", ""),
            ("BodyPart", "string", ""),
            ("Target", "string", ""),
            ("Category", "string", ""),
        ],
        "pos": (-3, -8),
        "color": "#16213e",
        "accent": "#38bdf8",
    },
    "taskexercises": {
        "columns": [
            ("Id", "int", "PK"),
            ("TaskId", "int", "FK -> tasks"),
            ("ExerciseId", "int", "FK -> exercises"),
            ("Sets", "int", "default 3"),
            ("Reps", "int", "default 10"),
            ("Weight", "float?", "kg"),
            ("RestSeconds", "int", "default 60"),
            ("IsCompleted", "bool", "default false"),
            ("OrderIndex", "int", "default 0"),
        ],
        "pos": (3, -8),
        "color": "#0f172a",
        "accent": "#94a3b8",
    },
}

# Relaciones: (tabla_origen, tabla_destino, etiqueta, tipo_flecha)
RELATIONS = [
    ("users", "purchases", "1:N", "->"),
    ("users", "plans", "1:N", "->"),
    ("users", "subscriptions", "1:N", "->"),
    ("users", "tasks", "1:N", "->"),
    ("users", "emailverifications", "1:N", "->"),
    ("users", "notificationpreferences", "1:1", "->"),
    ("users", "usersrooms", "1:N", "->"),
    ("users", "userachievements", "1:N", "->"),
    ("users", "bodymetrics", "1:N", "->"),
    ("items", "purchases", "1:N", "->"),
    ("rooms", "usersrooms", "1:N", "->"),
    ("tasks", "taskexercises", "1:N", "->"),
    ("exercises", "taskexercises", "1:N", "->"),
    ("achievements", "userachievements", "1:N", "->"),
    ("users", "items", "0:1", "->"),  # equippedStrengthId
    ("users", "items", "0:1", "->"),  # equippedEnduranceId
]


def draw_table(ax, name, data, x, y):
    """Dibuja una tabla como caja con header y columnas."""
    color = data["color"]
    accent = data["accent"]
    columns = data["columns"]

    # Dimensiones
    col_width = 3.2
    row_height = 0.35
    header_height = 0.55
    total_height = header_height + len(columns) * row_height + 0.1

    # Caja principal (con bordes redondeados)
    box = FancyBboxPatch(
        (x - col_width / 2, y - total_height),
        col_width,
        total_height,
        boxstyle="round,pad=0.05,rounding_size=0.15",
        facecolor=color,
        edgecolor=accent,
        linewidth=2,
        zorder=2,
    )
    ax.add_patch(box)

    # Header
    header = FancyBboxPatch(
        (x - col_width / 2, y - header_height),
        col_width,
        header_height,
        boxstyle="round,pad=0.05,rounding_size=0.15",
        facecolor=accent,
        edgecolor=accent,
        linewidth=0,
        zorder=3,
    )
    ax.add_patch(header)

    # Nombre de tabla en header
    ax.text(
        x,
        y - header_height / 2,
        name,
        ha="center",
        va="center",
        fontsize=11,
        fontweight="bold",
        color="black",
        zorder=4,
    )

    # Columnas
    for i, (col_name, col_type, extra) in enumerate(columns):
        yy = y - header_height - (i + 0.5) * row_height - 0.05
        # Linea separadora
        if i > 0:
            ax.plot(
                [x - col_width / 2 + 0.1, x + col_width / 2 - 0.1],
                [yy + row_height / 2, yy + row_height / 2],
                color=accent,
                linewidth=0.5,
                alpha=0.3,
                zorder=3,
            )
        # Nombre columna
        ax.text(
            x - col_width / 2 + 0.15,
            yy,
            col_name,
            ha="left",
            va="center",
            fontsize=7.5,
            color="white",
            fontfamily="monospace",
            zorder=4,
        )
        # Tipo + extra
        type_text = col_type
        if extra:
            type_text += f"  ({extra})"
        ax.text(
            x + col_width / 2 - 0.15,
            yy,
            type_text,
            ha="right",
            va="center",
            fontsize=6.5,
            color="#94a3b8",
            fontfamily="monospace",
            zorder=4,
        )

    return (x, y - total_height / 2, col_width, total_height)


def draw_relation(ax, t1_center, t2_center, label, arrow_style):
    """Dibuja una flecha entre dos tablas."""
    x1, y1 = t1_center
    x2, y2 = t2_center

    # Calcular puntos de borde (aproximados)
    dx = x2 - x1
    dy = y2 - y1
    dist = (dx ** 2 + dy ** 2) ** 0.5
    if dist == 0:
        return

    # Offset para que la flecha no entre dentro de la caja
    offset = 1.6
    ux, uy = dx / dist, dy / dist
    start = (x1 + ux * offset, y1 + uy * offset)
    end = (x2 - ux * offset, y2 - uy * offset)

    ax.annotate(
        "",
        xy=end,
        xytext=start,
        arrowprops=dict(
            arrowstyle="->",
            color="#64748b",
            lw=1.2,
            connectionstyle="arc3,rad=0.1",
        ),
        zorder=1,
    )

    # Etiqueta en el centro
    mx, my = (x1 + x2) / 2, (y1 + y2) / 2
    ax.text(
        mx,
        my + 0.2,
        label,
        ha="center",
        va="bottom",
        fontsize=7,
        color="#ffcc00",
        fontweight="bold",
        bbox=dict(boxstyle="round,pad=0.2", facecolor="#1a1a1a", edgecolor="#ffcc00", alpha=0.9),
        zorder=5,
    )


def main():
    print("Generando diagrama ER de TheTrainingHub...")

    fig, ax = plt.subplots(figsize=(22, 16), facecolor="#0f0c29")
    ax.set_facecolor("#0f0c29")
    ax.set_xlim(-10, 13)
    ax.set_ylim(-11, 7)
    ax.axis("off")

    # Titulo
    ax.text(
        0.5,
        0.98,
        "THE TRAINING HUB  -  Diagrama Entidad-Relacion",
        transform=ax.transAxes,
        ha="center",
        va="top",
        fontsize=20,
        fontweight="bold",
        color="#ffcc00",
    )
    ax.text(
        0.5,
        0.95,
        "16 tablas  |  Relaciones principales documentadas",
        transform=ax.transAxes,
        ha="center",
        va="top",
        fontsize=10,
        color="#94a3b8",
    )

    # Dibujar tablas
    table_centers = {}
    for name, data in TABLES.items():
        x, y = data["pos"]
        cx, cy, w, h = draw_table(ax, name, data, x, y)
        table_centers[name] = (cx, cy)

    # Dibujar relaciones (evitar duplicados exactos)
    drawn = set()
    for t1, t2, label, style in RELATIONS:
        key = tuple(sorted([t1, t2])) + (label,)
        if key in drawn:
            continue
        drawn.add(key)
        if t1 in table_centers and t2 in table_centers:
            draw_relation(ax, table_centers[t1], table_centers[t2], label, style)

    # Leyenda
    legend_items = [
        ("#ffcc00", "Entidad principal"),
        ("#38bdf8", "Catalogo / Item"),
        ("#34d399", "Transaccion / Historial"),
        ("#f472b6", "Configuracion / Preferencia"),
        ("#fbbf24", "Gamificacion / Logros"),
        ("#94a3b8", "Intermedia N:M"),
    ]
    leg_x, leg_y = 0.02, 0.02
    for i, (color, text) in enumerate(legend_items):
        ax.add_patch(
            plt.Rectangle((leg_x, leg_y + i * 0.025), 0.015, 0.015, transform=ax.transAxes, facecolor=color)
        )
        ax.text(
            leg_x + 0.02,
            leg_y + i * 0.025 + 0.0075,
            text,
            transform=ax.transAxes,
            fontsize=8,
            color="white",
            va="center",
        )

    plt.tight_layout()
    plt.savefig(OUTPUT_FILE, dpi=200, bbox_inches="tight", facecolor="#0f0c29", pad_inches=0.3)
    plt.close()

    print(f"✅ Diagrama guardado en: {OUTPUT_FILE}")
    print(f"📊 Tablas dibujadas: {len(TABLES)}")
    print(f"🔗 Relaciones dibujadas: {len(drawn)}")


if __name__ == "__main__":
    main()
