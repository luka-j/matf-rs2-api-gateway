export const frontends = [
  {
    name: "API 1",
    path: "/api/v1/",
    status: "up" as const,
  },
  {
    name: "API 2",
    path: "/api/v2/",
    status: "warn" as const,
  },
  {
    name: "API 3",
    path: "/api/v3/",
    status: "down" as const,
  },
];

export const backends = [
  {
    name: "API 1",
    path: "/api/v1/",
    status: "up" as const,
  },
  {
    name: "API 2",
    path: "/api/v2/",
    status: "warn" as const,
  },
  {
    name: "API 3",
    path: "/api/v3/",
    status: "down" as const,
  },
];

export const caches = [
  {
    type: "Redis",
    url: "redis://localhost:6379",
  },
  {
    type: "Memcached",
    url: "memcached://localhost:11211",
  },
  {
    type: "enchache",
    url: "enchache://localhost:11211",
  },
];

export const databases = [
  {
    type: "MySQL",
    url: "mysql://localhost:3306",
  },
  {
    type: "PostgreSQL",
    url: "postgresql://localhost:5432",
  },
  {
    type: "MongoDB",
    url: "mongodb://localhost:27017",
  },
];

export const queues = [
  {
    type: "RabbitMQ",
    url: "amqp://localhost:5672",
  },
  {
    type: "Kafka",
    url: "kafka://localhost:9092",
  },
];

export const apis = [
  {
    name: "API 1",
    lastConfigUpdate: "2021-08-01 12:00:00",
  },
  {
    name: "API 2",
    lastConfigUpdate: "2021-08-01 12:00:00",
  },
  {
    name: "API 3",
    lastConfigUpdate: "2021-08-01 12:00:00",
  },
];

export const rps = [
  {
    name: "RP 1",
    lastConfigUpdate: "2021-08-01 12:00:00",
  },
  {
    name: "RP 2",
    lastConfigUpdate: "2021-08-01 12:00:00",
  },
];

export type FrontBack = (typeof frontends)[number];

export type Cache = (typeof caches)[number];

export type System = (typeof apis)[number];
