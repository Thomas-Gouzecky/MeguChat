import { defineConfig } from 'vitest/config';

export default defineConfig({
  resolve: {
    tsconfigPaths: true,
  },

  test: {
    globals: true,
    environment: 'jsdom',
    include: ['vitest/**/*.{test,spec}.{ts,tsx}', 'vitest/**/test_*.{ts,tsx}'],

    setupFiles: ['./vitest/setup.ts'],

    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
    },
  },
});
