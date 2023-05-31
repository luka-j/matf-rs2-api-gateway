import DatasourceOverview from "./components/DatasourceOverview";
import FrontBackOverview from "./components/FrontBackOverview";
import SystemOverview from "./components/SystemOverview";

const frontends = [
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

const backends = [
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

const caches = [
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

const databases = [
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

const queues = [
  {
    type: "RabbitMQ",
    url: "amqp://localhost:5672",
  },
  {
    type: "Kafka",
    url: "kafka://localhost:9092",
  },
];

const apis = [
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

const rps = [
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

const Dashboard = () => {
  return (
    <main className="mt-8 mx-auto max-w-7xl">
      <h1 className="text-4xl text-center font-bold text-white">API Gateway Dashboard</h1>

      <div className="mt-8 block w-full p-6  border  rounded-lg shadow  bg-gray-800 border-gray-700">
        <h2 className="mb-2 text-3xl font-semibold text-white">Config Overview</h2>
        <p className="font-normal  text-gray-400">Here you can preview and edit your configs.</p>
        <div className="flex justify-between items-center flex-col md:flex-row">
          <FrontBackOverview frontBackList={frontends} type="Frontends" up={5} total={7} />
          <FrontBackOverview frontBackList={backends} type="Backends" up={5} total={5} />
        </div>
      </div>

      <div className="mt-8 block w-full p-6  border  rounded-lg shadow  bg-gray-800 border-gray-700">
        <div className="flex justify-between">
          <div>
            <h5 className="mb-2 text-3xl font-semibold text-white">Datasources</h5>
            <p className="font-normal  text-gray-400">
              Here you can preview and edit your datasources.
            </p>
          </div>
          <a
            href="/dashboard/datasources"
            className="focus:ring-4 focus:outline-none font-medium rounded-lg text-sm h-10 px-4 py-2 text-center mr-3 md:mr-0 bg-blue-600 hover:bg-blue-700 focus:ring-blue-800"
          >
            See details
          </a>
        </div>
        <div className="flex justify-between items-center flex-col md:flex-row gap-12">
          <DatasourceOverview datasourceList={caches} title="Caches" />
          <DatasourceOverview datasourceList={databases} title="Databases" />
          <DatasourceOverview datasourceList={queues} title="Queues" />
        </div>
      </div>

      <div className="mt-8 block w-full p-6  border  rounded-lg shadow  bg-gray-800 border-gray-700">
        <h2 className="mb-2 text-3xl font-semibold text-white">System Overview</h2>
        <p className="font-normal  text-gray-400">Here you can preview and edit your systems.</p>
        <div className="flex justify-between items-center flex-col md:flex-row">
          <SystemOverview systemList={apis} instanceAmount={3} title="API" />
          <SystemOverview systemList={rps} instanceAmount={2} title="RP" />
        </div>
      </div>
    </main>
  );
};

export default Dashboard;
