import { AiFillPlusCircle } from "react-icons/ai";

import { Datasource } from "..";
import DatasourceCard from "./DatasourceCard";

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

export default DatasourceCardsWrapper;
