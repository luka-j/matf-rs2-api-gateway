import { Cache } from "..";

interface IDatasourceOverviewProps {
  datasourceList: Cache[];
  title: string;
}

const DatasourceOverview = ({ datasourceList, title }: IDatasourceOverviewProps) => {
  return (
    <div className="mt-12 w-full max-w-md p-4  border  rounded-lg shadow sm:p-8 bg-gray-800 border-gray-600">
      <div className="flex items-center justify-between mb-4">
        <h5 className="text-xl font-bold leading-none  text-white">{title.toUpperCase()}</h5>
      </div>
      <div className="flow-root">
        <ul className="divide-y  divide-gray-700">
          {datasourceList.map((datasource, index) => (
            <li key={index} className="py-3 sm:py-4 hover:bg-gray-700 px-2 rounded-lg text-white">
              <span className="font-semibold">{datasource.type}</span> on{" "}
              <span className="text-blue-500">{datasource.url}</span>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export default DatasourceOverview;
