export const caches = [
  {
    id: 1,
    name: "Cache 1",
    type: "Redis",
    url: "redis://localhost:6379",
    username: "username@admin",
  },
  {
    id: 2,
    name: "Cache 2",
    type: "Memcached",
    url: "memcached://localhost:11211",
    username: "username@admin",
  },
  {
    id: 3,
    name: "Cache 3",
    type: "enchache",
    url: "enchache://localhost:11211",
    username: "username@admin",
  },
];

export const dbs = [
  {
    id: 1,
    name: "DB 1",
    type: "MySQL",
    url: "mysql://localhost:3306",
    username: "username@admin",
  },
  {
    id: 2,
    name: "DB 2",
    type: "PostgreSQL",
    url: "postgresql://localhost:5432",
    username: "username@admin",
  },
];

export const queues = [
  {
    id: 1,
    name: "Queue 1",
    type: "RabbitMQ",
    url: "rabbitmq://localhost:5672",
    username: "username@admin",
  },
];

export type DataSource = (typeof caches)[0];
