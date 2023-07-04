import React from "react";

import Modal from "@/components/Modal";

import AddModalContent from "./components/AddModalContent";
import DatasourceCardsWrapper from "./components/DatasourceCardsWrapper";

const caches = [
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

const dbs = [
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

const queues = [
  {
    id: 1,
    name: "Queue 1",
    type: "RabbitMQ",
    url: "rabbitmq://localhost:5672",
    username: "username@admin",
  },
];

export type Datasource = (typeof caches)[0];

const Datasources = () => {
  const [showAddModalType, setShowAddModalType] = React.useState("");

  return (
    <main className="mt-8 mx-auto max-w-7xl pb-8">
      <h1 className="text-4xl text-center font-bold ">Datasources</h1>

      <DatasourceCardsWrapper
        type="cache"
        datasources={caches}
        setShowAddModalType={setShowAddModalType}
      />

      <DatasourceCardsWrapper
        type="database"
        datasources={dbs}
        setShowAddModalType={setShowAddModalType}
      />

      <DatasourceCardsWrapper
        type="queue"
        datasources={queues}
        setShowAddModalType={setShowAddModalType}
      />

      <Modal
        tabIndex={0}
        open={Boolean(showAddModalType)}
        onClose={() => setShowAddModalType("")}
        className="max-w-xl h-fit bg-slate-700"
      >
        <AddModalContent type={showAddModalType} handleClose={() => setShowAddModalType("")} />
      </Modal>
    </main>
  );
};

export default Datasources;
