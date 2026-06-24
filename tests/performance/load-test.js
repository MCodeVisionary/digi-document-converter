import http from 'k6/http';
import { check, sleep } from 'k6';
const BASE = __ENV.PERF_BASE_URL || 'http://localhost:8080';
export const options = {
  vus: Number(__ENV.VUS || 30), duration: __ENV.DURATION || '60s',
  thresholds: { http_req_duration: ['p(95)<500'], http_req_failed: ['rate<0.01'] },
};
export default function () {
  const payload = JSON.stringify({ filename: 'a.txt', sourceType: 'transcript', text: 'load test', target: 'json' });
  const res = http.post(`${BASE}/api/convert`, payload, { headers: { 'Content-Type': 'application/json' } });
  check(res, { 'status 200': (r) => r.status === 200 });
  sleep(0.5);
}
