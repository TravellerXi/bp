import { test, expect } from '@playwright/test';

type Case = { systolic: number; diastolic: number; category: string };

// Same acceptance pairs as the unit and BDD suites, but exercised through the
// real Razor page on a deployed environment.
const cases: Case[] = [
  { systolic: 150, diastolic: 90, category: 'High Blood Pressure' },
  { systolic: 120, diastolic: 89, category: 'Pre-High Blood Pressure' },
  { systolic: 110, diastolic: 70, category: 'Ideal Blood Pressure' },
  { systolic: 80, diastolic: 65, category: 'Ideal Blood Pressure' },
  { systolic: 89, diastolic: 50, category: 'Low Blood Pressure' },
];

const submit = async (page, systolic: number, diastolic: number) => {
  await page.goto('/');
  await page.fill('#BP_Systolic', String(systolic));
  await page.fill('#BP_Diastolic', String(diastolic));
  await page.click('input[type="submit"]');
};

const isHttps = (process.env.BASE_URL ?? '').startsWith('https');

// A deployed App Service serves http and https on the default ports, so swapping
// the scheme is enough. A local Kestrel instance listens on two different ports,
// so the plain-text origin has to be supplied explicitly via HTTP_BASE_URL.
const httpOrigin = (baseURL: string) =>
  process.env.HTTP_BASE_URL ?? baseURL.replace(/^https:/, 'http:').replace(/:\d+$/, '');

test.describe('BP category calculator', () => {
  test('the form renders', async ({ page }) => {
    const response = await page.goto('/');
    expect(response?.status()).toBe(200);
    await expect(page.getByRole('heading', { name: /BP Category Calculator/i })).toBeVisible();
  });

  test('http is redirected to https', async ({ request, baseURL }) => {
    test.skip(!isHttps, 'Only meaningful against an https environment');
    const res = await request.get(httpOrigin(baseURL!), { maxRedirects: 0 });
    expect([301, 302, 307, 308]).toContain(res.status());
    expect(res.headers()['location']).toMatch(/^https:/);
  });

  test('security headers are present', async ({ request }) => {
    const res = await request.get('/');
    const h = res.headers();
    expect(h['x-frame-options']).toBe('DENY');
    expect(h['x-content-type-options']).toBe('nosniff');
    expect(h['content-security-policy']).toContain("default-src 'self'");
    expect(h['cache-control']).toContain('no-store');
    expect(h['server']).toBeUndefined();
  });

  for (const c of cases) {
    test(`${c.systolic}/${c.diastolic} is reported as ${c.category}`, async ({ page }) => {
      await submit(page, c.systolic, c.diastolic);
      await expect(page.locator('form#form1')).toContainText(c.category);
    });
  }

  test('systolic must be greater than diastolic', async ({ page }) => {
    await submit(page, 90, 95);
    await expect(page.locator('div[class*="validation-summary"]'))
      .toContainText('Systolic must be greater than Diastolic');
  });

  test('out-of-range systolic is rejected', async ({ page }) => {
    await submit(page, 250, 80);
    await expect(page.locator('[data-valmsg-for="BP.Systolic"]'))
      .toContainText('Invalid Systolic Value');
  });
});

test.describe('Mean arterial pressure', () => {
  const mapCases = [
    { systolic: 120, diastolic: 80, map: '93.3', band: 'Normal Mean Arterial Pressure' },
    { systolic: 150, diastolic: 90, map: '110', band: 'High Mean Arterial Pressure' },
    { systolic: 89, diastolic: 60, map: '69.7', band: 'Low Mean Arterial Pressure' },
  ];

  for (const c of mapCases) {
    test(`${c.systolic}/${c.diastolic} reports a MAP of ${c.map}`, async ({ page }) => {
      await submit(page, c.systolic, c.diastolic);
      await expect(page.getByTestId('map-value')).toContainText(c.map);
      await expect(page.getByTestId('map-category')).toContainText(c.band);
    });
  }
});
