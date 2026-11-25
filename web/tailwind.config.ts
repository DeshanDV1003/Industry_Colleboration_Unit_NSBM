import type { Config } from 'tailwindcss'

/** Tailwind maps onto the CSS variables in src/styles/globals.css so light/dark themes switch in one place. */
const token = (name: string) => `var(--${name})`

export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  darkMode: ['selector', '[data-theme="dark"]'],
  theme: {
    extend: {
      colors: {
        primary: { DEFAULT: token('color-primary'), hover: token('color-primary-hover'), soft: token('color-primary-soft') },
        accent: { DEFAULT: token('color-accent'), soft: token('color-accent-soft') },
        success: { DEFAULT: token('color-success'), soft: token('color-success-soft') },
        warning: { DEFAULT: token('color-warning'), soft: token('color-warning-soft') },
        danger: { DEFAULT: token('color-danger'), soft: token('color-danger-soft') },
        heading: token('color-heading'),
        body: token('color-text'),
        muted: token('color-text-muted'),
        page: token('color-bg-page'),
        surface: { DEFAULT: token('color-bg-card'), subtle: token('color-bg-subtle'), sidebar: token('color-bg-sidebar') },
        line: { DEFAULT: token('color-border'), strong: token('color-border-strong') },
        code: { DEFAULT: token('color-code-bg'), text: token('color-code-text'), muted: token('color-code-muted'), line: token('color-code-border') },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        display: ['"Roboto Slab"', 'Georgia', 'serif'],
      },
      borderRadius: {
        card: token('radius-card'),
        control: token('radius-control'),
      },
      boxShadow: {
        card: token('shadow-card'),
        pop: token('shadow-pop'),
      },
      backgroundImage: {
        brand: token('gradient-brand'),
      },
    },
  },
  plugins: [],
} satisfies Config
