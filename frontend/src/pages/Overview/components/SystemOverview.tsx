import { System } from "..";

interface ISystemOverviewProps {
  systemList: System[];
  title: string;
  instanceAmount: number;
}

const SystemOverview = ({ systemList, title, instanceAmount }: ISystemOverviewProps) => {
  return (
    <div className="mt-12 w-full max-w-md p-4  border  rounded-lg shadow sm:p-8 bg-gray-800 border-gray-600">
      <div className="flex items-center justify-between mb-4">
        <h5 className="text-xl font-bold leading-none  text-white">
          {title.toUpperCase()}: {instanceAmount} instances
        </h5>
      </div>
      <div className="flow-root">
        <ul className="divide-y  divide-gray-700">
          {systemList.map((system, index) => (
            <li key={index} className="py-3 sm:py-4 hover:bg-gray-700 px-2 rounded-lg text-white">
              <span className="font-semibold">{system.name}</span> last config update at{" "}
              <span className="">{system.lastConfigUpdate}</span>
            </li>
          ))}
        </ul>
      </div>
      <div className="flex items-center justify-end mt-4">
        <a href="/" className="text-lg font-medium hover:underline text-blue-500">
          See metrics...
        </a>
      </div>
    </div>
  );
};

export default SystemOverview;
