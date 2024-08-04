import http from 'k6/http';
import { sleep } from 'k6';
export const options = {
    vus: 500,
    duration: '30s',
  };
export default function () {
  http.get('https://localhost:7159/api/v1/Utility/update-country-list');
  sleep(1);
}