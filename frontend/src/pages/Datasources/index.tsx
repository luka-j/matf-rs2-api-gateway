import React from "react";
import { AiFillPlusCircle } from "react-icons/ai";

import Modal from "@/components/Modal";

import AddModalContent from "./components/AddModalContent";
import DatasourceCard from "./components/DatasourceCard";

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

const DatasourceCardsWrapper = ({
  type,
  datasources,
  setShowAddModalType,
}: {
  type: string;
  datasources: Datasource[];
  setShowAddModalType: React.Dispatch<React.SetStateAction<string>>;
}) => {
  return (
    <div className="mt-8 block w-full p-6  border  rounded-lg shadow  bg-gray-800 border-gray-700">
      <h2 className="mb-2 text-3xl font-semibold capitalize items-center justify-center inline-flex gap-4">
        {type}s
        <div
          className="mt-1 hover:bg-gray-600 rounded-full p-1 cursor-pointer transition-all duration-300 active:bg-transparent"
          onClick={() => setShowAddModalType(type)}
        >
          <AiFillPlusCircle />
        </div>
      </h2>
      <p className="font-normal  text-gray-400">Here you can preview and edit your {type}s.</p>
      <div className="flex justify-between items-center flex-col md:flex-row flex-wrap">
        {datasources.map((datasource) => (
          <DatasourceCard key={datasource.id} {...datasource} />
        ))}
      </div>
    </div>
  );
};
