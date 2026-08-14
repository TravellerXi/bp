import { check, sleep } from 'k6';
import http from 'k6/http';
import { randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';

// Ramp-up / plateau / ramp-down, following the load pattern used in the module's
// k6 samples. The threshold makes the test itself a quality gate.
export const options = {
  stages: [
    { duration: '1m', target: 20 },
    { duration: '1m', target: 20 },
    { duration: '1m', target: 0 },
  ],
  thresholds: {
    http_req_duration: ['p(95)<800'],
    http_req_failed: ['rate<0.01'],
    checks: ['rate>0.99'],
  },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5000';

// The Razor page posts an anti-forgery token, so each iteration GETs the form,
// scrapes the token and cookie, then POSTs a random but valid reading.
export default function () {
  const get = http.get(`${BASE_URL}/`);
  check(get, { 'form loaded': (r) => r.status === 200 });

  const token = get.html().find('input[name=__RequestVerificationToken]').first().attr('value');

  // Stay inside the model's validation ranges and keep systolic > diastolic.
  const diastolic = randomIntBetween(40, 99);
  const systolic = randomIntBetween(diastolic + 1, 190);

  const res = http.post(
    `${BASE_URL}/`,
    {
      'BP.Systolic': String(systolic),
      'BP.Diastolic': String(diastolic),
      __RequestVerificationToken: token,
    },
    { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } },
  );

  check(res, {
    'post succeeded': (r) => r.status === 200,
    'a category was rendered': (r) => /Blood Pressure/.test(r.body),
  });

  sleep(randomIntBetween(1, 3));
}
