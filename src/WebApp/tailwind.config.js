/** @type {import('tailwindcss').Config} */
export default {
  content: ['app.vue', './pages/**/*.{html,js,vue,ts}', './modules/**/*.{html,js,vue,ts}', './components/**/*.vue'],
  theme: {
    extend: {},
  },
  plugins: [require('daisyui')],
}
