import { defineConfig, devices } from '@playwright/test';

// BASE_URL is supplied by the pipeline and points at the environment that was
// just deployed, so the same suite runs against dev, qa or the staging slot.
const baseURL = process.env.BASE_URL ?? 'http://localhost:5000';

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [['html', { outputFolder: 'playwright-report' }], ['list']],
  use: {
    baseURL,
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    // The ASP.NET Core dev certificate is self-signed when running locally.
    ignoreHTTPSErrors: baseURL.includes('localhost'),
  },
  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
    { name: 'firefox', use: { ...devices['Desktop Firefox'] } },
    { name: 'webkit', use: { ...devices['Desktop Safari'] } },
  ],
});
