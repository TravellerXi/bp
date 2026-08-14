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

test.describe('BP category calculator', () => {
  test('the form is served over HTTPS and renders', async ({ page }) => {
    const response = await page.goto('/');
    expect(response?.status()).toBe(200);
    expect(page.url()).toMatch(/^https:/);
    await expect(page.getByRole('heading', { name: /BP Category Calculator/i })).toBeVisible();
  });

  for (const c of cases) {
    test(`${c.systolic}/${c.diastolic} is reported as ${c.category}`, async ({ page }) => {
      await submit(page, c.systolic, c.diastolic);
      await expect(page.locator('form#form1')).toContainText(c.category);
    });
  }

  test('systolic must be greater than diastolic', async ({ page }) => {
    await submit(page, 90, 95);
    await expect(page.locator('.text-danger')).toContainText('Systolic must be greater than Diastolic');
  });

  test('out-of-range systolic is rejected', async ({ page }) => {
    await submit(page, 250, 80);
    await expect(page.locator('.text-danger')).toContainText('Invalid Systolic Value');
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
