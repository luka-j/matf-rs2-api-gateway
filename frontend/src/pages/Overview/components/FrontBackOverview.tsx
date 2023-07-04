import { AiOutlineWarning } from "react-icons/ai";
import { FcCheckmark } from "react-icons/fc";
import { VscError } from "react-icons/vsc";

import { FrontBack } from "..";

const statusMapping = {
  up: {
    icon: FcCheckmark,
    color: "text-green-500",
  },
  warn: {
    icon: AiOutlineWarning,
    color: "text-yellow-500",
  },
  down: {
    icon: VscError,
    color: "text-red-500",
  },
};

interface IFrontBackOverviewProps {
  frontBackList: FrontBack[];
  type: string;
  up: number;
  total: number;
}

const FrontBackOverview = ({ frontBackList, type, up, total }: IFrontBackOverviewProps) => {
  return (
    <div className="mt-12 w-full max-w-md p-4  border  rounded-lg shadow sm:p-8 bg-gray-800 border-gray-700">
      <div className="flex items-center justify-between mb-4">
        <h5 className="text-xl font-bold leading-none  ">
          {type.toUpperCase()}: {up}/{total} up
        </h5>
        <a
          href="/dashboard/frontends"
          className="text-sm font-medium hover:underline text-blue-500"
        >
          View all
        </a>
      </div>
      <div className="flow-root">
        <ul className="divide-y  divide-gray-700">
          {frontBackList.map((frontBack, index) => (
            <li key={index} className="py-3 sm:py-4 hover:bg-gray-700 px-2 rounded-lg">
              <div className="flex items-center space-x-4">
                <div className={`flex-shrink-0 ${statusMapping[frontBack.status].color}`}>
                  {statusMapping[frontBack.status].icon({
                    className: "h-6 w-6",
                  })}
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium truncate ">{frontBack.name}</p>
                  <p className="text-sm  truncate text-gray-400">{frontBack.path}</p>
                </div>
                <a
                  href={`/dashboard/frontends/api1${frontBack.path}`}
                  className="text-sm font-medium hover:underline text-blue-500"
                >
                  View spec
                </a>
              </div>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export default FrontBackOverview;
